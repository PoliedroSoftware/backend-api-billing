using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations
{
    public class DianResolutionConfiguration
    {
        public DianResolutionConfiguration(EntityTypeBuilder<DianResolutionEntity> builder)
        {
            builder.ToTable("dian_resolution_billing");
            builder.HasKey(x => x.ResolutionId);
            builder.Property(x => x.ResolutionId).HasColumnName("resolution_id");
            builder.Property(x => x.CompanyProviderId).HasColumnName("company_provider_id");
            builder.Property(x => x.ResolutionType).HasConversion(ConvertResolutionType()).HasColumnName("document_type");
            builder.Property(x => x.Description).HasColumnName("Description");
            builder.Property(x => x.ResolutionNumber).HasColumnName("resolution_number");
            builder.Property(x => x.Prefix).HasColumnName("prefix");
            builder.Property(x => x.MultipleResolution).HasColumnName("multiple_resolution");
            builder.Property(x => x.VigencyMonth).HasColumnName("vigency_month");
            builder.Property(x => x.Automatic).HasColumnName("automatic");
            builder.Property(x => x.InitialRange).HasColumnName("initial_range");
            builder.Property(x => x.FinalRange).HasColumnName("final_range");
            builder.Property(x => x.CurrentRange).HasColumnName("current_number");
            builder.Property(x => x.ResolutionDate).HasColumnName("resolution_date");
            builder.Property(x => x.ExpirationDate).HasColumnName("expiration_date");
            builder.Property(x => x.ExpirationDays).HasColumnName("expiration_days");
            builder.Property(x => x.ExpirationNumber).HasColumnName("expiration_numbers");
            builder.Property(x => x.Active).HasColumnName("active");
        }

        private ValueConverter ConvertResolutionType()
        {
            return new ValueConverter<ResolutionType, string>(
                v => v.ToString().ToUpper(),
                v => (ResolutionType)Enum.Parse(typeof(ResolutionType), v, true));
        }
    }
}
