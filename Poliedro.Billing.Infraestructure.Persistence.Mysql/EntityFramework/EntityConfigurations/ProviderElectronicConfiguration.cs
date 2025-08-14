using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.Provider.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;

public class ProviderElectronicConfiguration
{
    public ProviderElectronicConfiguration(EntityTypeBuilder<ProviderEntity> builder)
    {
        builder.ToTable("provider");
        builder.HasKey(x => x.ProviderId);
        builder.Property(x => x.ProviderId).HasColumnName("providerid");
        builder.Property(x => x.Description).HasColumnName("description")
            .HasMaxLength(100)
            .IsRequired();
    }
}