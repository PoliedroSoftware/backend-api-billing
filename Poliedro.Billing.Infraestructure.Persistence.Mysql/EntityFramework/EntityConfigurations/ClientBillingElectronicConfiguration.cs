using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations
{
    public class ClientBillingElectronicConfiguration
    {
        public ClientBillingElectronicConfiguration(EntityTypeBuilder<ClientEntity> builder)
        {
            builder.ToTable("billing_company");
            builder.HasKey(x => x.CompanyId);
            builder.Property(x => x.CompanyId).HasColumnName("company_id");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.Nit).HasColumnName("nit").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.Active).HasColumnName("active");
        }
    }
}