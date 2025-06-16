using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations
{
    public class ClientBillingElectronicConfiguration
    {
        public ClientBillingElectronicConfiguration(EntityTypeBuilder<ClientEntity> builder)
        {
            builder.ToTable("client_billing_electronic");
            builder.HasKey(x => x.ClientBillingElectronicId);
            builder.Property(x => x.ClientBillingElectronicId).HasColumnName("client_billing_electronic_id");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.ResolutionId).HasColumnName("resolutionid");
            builder.Property(x => x.ServerId).HasColumnName("serverid");
            builder.Property(x => x.ProviderId).HasColumnName("providerid");
            builder.Property(x => x.Active).HasColumnName("active");
            builder.Property(x => x.Iterations).HasColumnName("iterations");
            builder.Property(x => x.Date).HasColumnName("date");
            builder.Property(x => x.ApiKey).HasColumnName("apikey");
            builder.Property(x => x.Automatic).HasColumnName("automatic");
            builder.Property(x => x.MultipleResolution).HasColumnName("multiple_resolution");
            builder.Property(x => x.Email).HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired(false);
            builder.HasOne(x => x.Server)
                .WithMany(x => x.clientsBillingElectronic)
                .HasForeignKey(x => x.ServerId);

            builder.HasOne(x => x.DianResolution)
                .WithMany(x => x.clientsBillingElectronic)
                .HasForeignKey(x => x.ResolutionId);
        }
    }
}