
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.CompanyProvider.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;
public class CompanyProviderConfiguration
{
    public CompanyProviderConfiguration(EntityTypeBuilder<CompanyProviderEntity> builder)
    {
        builder.ToTable("company_provider");
        builder.HasKey(x => x.CompanyProviderId);
        builder.Property(x => x.CompanyProviderId).HasColumnName("company_provider_id");
        builder.Property(x => x.CompanyId).HasColumnName("company_id");
        builder.Property(x => x.ProviderId).HasColumnName("provider_id");
        //*
        builder.Property(x => x.ServiceId).HasColumnName("server_id");

        builder.Property(x => x.ApiUser).HasColumnName("api_user").HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.ApiPassword).HasColumnName("api_password").HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.ApiKey).HasColumnName("api_key").HasMaxLength(100).IsRequired(false);
        //*
        builder.Property(x => x.EnvironmentType).HasColumnName("environment_type").HasConversion<string>();

        builder.Property(x => x.HeadNote).HasColumnName("head_note").HasMaxLength(1000).IsRequired(false);
        //*
        builder.Property(x => x.FooterNote).HasColumnName("foot_note").HasMaxLength(1000).IsRequired(false);

        builder.Property(x => x.Active).HasColumnName("active");

    }
}
