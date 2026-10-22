using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.Billing;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;

public sealed class EmissionFailureConfiguration : IEntityTypeConfiguration<EmissionFailure>
{
    public void Configure(EntityTypeBuilder<EmissionFailure> builder)
    {
        builder.ToTable("billing_emission_failure").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.CompanyId).HasColumnName("company_id");
        builder.Property(x => x.ResolutionId).HasColumnName("resolution_id");
        builder.Property(x => x.ClientName).HasColumnName("client_name").HasMaxLength(200);
        builder.Property(x => x.InvoiceNumber).HasColumnName("invoice_number").HasMaxLength(100);
        builder.Property(x => x.Prefix).HasColumnName("prefix").HasMaxLength(30);
        builder.Property(x => x.DocumentType).HasColumnName("document_type").HasMaxLength(20);
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(2000);
        builder.Property(x => x.FailedAt).HasColumnName("failed_at").HasColumnType("datetime(6)");
        builder.HasIndex(x => new { x.CompanyId, x.FailedAt }).HasDatabaseName("ix_billing_emission_failure_company_date");
    }
}
