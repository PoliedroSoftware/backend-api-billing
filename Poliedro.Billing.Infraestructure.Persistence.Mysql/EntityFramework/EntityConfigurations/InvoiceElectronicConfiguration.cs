using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;

public class InvoiceElectronicConfiguration
{
    public InvoiceElectronicConfiguration(EntityTypeBuilder<InvoiceElectronic> builder)
    {
        builder.ToTable("v_invoice_electronic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Identication).HasColumnName("identication");
        builder.Property(x => x.ContactName).HasColumnName("contact_name");
        builder.Property(x => x.Email).HasColumnName("email");
        builder.Property(x => x.Mobile).HasColumnName("mobile");
        builder.Property(x => x.City).HasColumnName("city");
        builder.Property(x => x.State).HasColumnName("state");
        builder.Property(x => x.Country).HasColumnName("country");
        builder.Property(x => x.Invoice).HasColumnName("invoice");
        builder.Property(x => x.PaymentStatus).HasColumnName("payment_status");
        builder.Property(x => x.TransactionDate).HasColumnName("transaction_date");
        builder.Property(x => x.AllowanceTotal).HasColumnName("allowanceTotal");
        builder.Property(x => x.InvoiceBaseTotal).HasColumnName("invoiceBaseTotal");
        builder.Property(x => x.InvoiceTaxExclusiveTotal).HasColumnName("invoiceTaxExclusiveTotal");
        builder.Property(x => x.InvoiceTaxInclusiveTotal).HasColumnName("invoiceTaxInclusiveTotal");
        builder.Property(x => x.TotalToPay).HasColumnName("totalToPay");
        builder.Property(x => x.CreatedByName).HasColumnName("created_by_name");
    }
}