using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.BillingPos;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;

public class InvoiceSuccessEntityConfiguration : IEntityTypeConfiguration<InvoiceSuccessEntity>
{
    public void Configure(EntityTypeBuilder<InvoiceSuccessEntity> builder)
    {
        builder.ToTable("invoice_success").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Invoice).HasColumnName("invoice");
        builder.Property(x => x.ProviderId).HasColumnName("providerid");
        builder.Property(x => x.DateRegister).HasColumnName("dateregister");
        builder.Property(x => x.Cude).HasColumnName("cude");
        builder.Property(x => x.QrCode).HasColumnName("qrCode");
        builder.Property(x => x.ClientId).HasColumnName("client_id");
    }
}
