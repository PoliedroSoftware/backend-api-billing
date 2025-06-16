@echo off
REM Instalar dotnet-sonarscanner globalmente (omite si ya está instalado)
dotnet tool install --global dotnet-sonarscanner

REM Iniciar análisis SonarCloud
dotnet sonarscanner begin /o:"poliedrocloud" /k:"poliedrocloud_Poliedro.Billing" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.token="606e98d70ddb6d7b02553f001cd55ad641df1e1d"

REM Construir el proyecto
dotnet build

REM Finalizar análisis SonarCloud
dotnet sonarscanner end /d:sonar.token="606e98d70ddb6d7b02553f001cd55ad641df1e1d"

pause
