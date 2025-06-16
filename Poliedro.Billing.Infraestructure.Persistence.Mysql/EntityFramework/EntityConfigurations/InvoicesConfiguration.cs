using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.BillingPos;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;

public class InvoicesConfiguration : IEntityTypeConfiguration<InvoiceEntity>
{
    public void Configure(EntityTypeBuilder<InvoiceEntity> builder)
    {
       
        builder.ToTable("v_api_invoice").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Date).HasColumnName("Date");
        builder.Property(x => x.Time).HasColumnName("Time");
        builder.Property(x => x.Resolution).HasColumnName("Resolution");
        builder.Property(x => x.Prefix).HasColumnName("Prefix");
        builder.Property(x => x.Number).HasColumnName("Number");
        builder.Property(x => x.Note).HasColumnName("Note");
        builder.Property(x => x.ResolutionType).HasColumnName("resolutionType");
        builder.Property(x => x.AllowanceTotal).HasColumnName("allowanceTotal");
        builder.Property(x => x.InvoiceBaseTotal).HasColumnName("invoiceBaseTotal");
        builder.Property(x => x.InvoiceTaxExclusiveTotal).HasColumnName("invoiceTaxExclusiveTotal");
        builder.Property(x => x.InvoiceTaxInclusiveTotal).HasColumnName("invoiceTaxInclusiveTotal");
        builder.Property(x => x.TotalToPay).HasColumnName("totalToPay");
        builder.Property(x => x.FinalTotalToPay).HasColumnName("finalTotalToPay");
    }
}
