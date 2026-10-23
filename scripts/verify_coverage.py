#!/usr/bin/env python3
"""Run Billing test suites and enforce line coverage for production assemblies."""

from __future__ import annotations

import subprocess
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
THRESHOLD = 80.0
TEST_PROJECTS = (
    "Poliedro.Billing.Api.Tests/Poliedro.Billing.Api.Tests.csproj",
    "Poliedro.Billing.Domain.Test/Poliedro.Billing.Domain.Test.csproj",
)
PRODUCTION_ASSEMBLIES = (
    "Poliedro.Billing.Api",
    "Poliedro.Billing.Application",
    "Poliedro.Billing.Domain",
    "Poliedro.Billing.Infraestructure.External.Plemsi",
    "Poliedro.Billing.Infraestructure.External.Siigo",
    "Poliedro.Billing.Infraestructure.External.TNS",
    "Poliedro.Billing.Infraestructure.Persistence.Mysql",
)


def latest_report(project: str) -> Path:
    reports = list((ROOT / Path(project).parent / "TestResults").glob("*/coverage.cobertura.xml"))
    if not reports:
        raise FileNotFoundError(f"No Cobertura report found for {project}")
    return max(reports, key=lambda report: report.stat().st_mtime)


def merge_reports(reports: list[Path]) -> dict[str, dict[tuple[str, str], bool]]:
    """Merge hit status by assembly and canonical source line, taking the union."""
    merged: dict[str, dict[tuple[str, str], bool]] = {
        assembly: {} for assembly in PRODUCTION_ASSEMBLIES
    }
    for report in reports:
        root = ET.parse(report).getroot()
        for package in root.findall(".//package"):
            assembly = package.get("name", "")
            if assembly not in merged:
                continue
            for source_class in package.findall("./classes/class"):
                filename = (source_class.get("filename") or "").replace("\\", "/")
                if assembly == "Poliedro.Billing.Domain" and filename.startswith("Poliedro.Billing.Domain/"):
                    filename = filename.removeprefix("Poliedro.Billing.Domain/")
                for line in source_class.findall("./lines/line"):
                    key = (filename, line.get("number", ""))
                    hit = int(line.get("hits", "0")) > 0
                    merged[assembly][key] = merged[assembly].get(key, False) or hit
    return merged


def main() -> int:
    reports: list[Path] = []
    for project in TEST_PROJECTS:
        command = [
            "dotnet",
            "test",
            str(ROOT / project),
            "--configuration",
            "Release",
            "--collect:XPlat Code Coverage",
        ]
        print(f"Running: {' '.join(command)}", flush=True)
        subprocess.run(command, cwd=ROOT, check=True)
        reports.append(latest_report(project))

    coverage = merge_reports(reports)
    failed: list[str] = []
    total_covered = total_lines = 0
    print("\nMerged line coverage by production assembly:")
    for assembly in PRODUCTION_ASSEMBLIES:
        lines = coverage[assembly]
        covered = sum(lines.values())
        count = len(lines)
        percent = covered * 100 / count if count else 0.0
        total_covered += covered
        total_lines += count
        print(f"  {assembly}: {covered}/{count} ({percent:.2f}%)")
        if percent <= THRESHOLD:
            failed.append(assembly)

    overall = total_covered * 100 / total_lines if total_lines else 0.0
    print(f"Combined distinct production lines: {total_covered}/{total_lines} ({overall:.2f}%)")
    if failed:
        print(f"Coverage must exceed {THRESHOLD:.0f}% for each assembly: {', '.join(failed)}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (OSError, subprocess.CalledProcessError, ET.ParseError) as error:
        print(f"Coverage verification failed: {error}", file=sys.stderr)
        raise SystemExit(1)
