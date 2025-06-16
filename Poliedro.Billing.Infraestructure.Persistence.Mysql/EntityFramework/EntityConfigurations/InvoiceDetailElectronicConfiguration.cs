
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;

public class InvoiceDetailElectronicConfiguration
{
	public InvoiceDetailElectronicConfiguration(EntityTypeBuilder<Domain.InvoiceDetailElectronic.Entities.InvoiceWithDetailElectronic> builder) {

        builder.ToTable("v_invoice_detail_electronic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Transaccion).HasColumnName("transaccion").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").IsRequired();
        builder.Property(x => x.TypeItemIdentification).HasColumnName("type_item_identification").IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").IsRequired(false);
        builder.Property(x => x.UnitMeasureId).HasColumnName("unit_measure_id").IsRequired();
        builder.Property(x => x.BaseQuantity).HasColumnName("base_quantity").IsRequired();
        builder.Property(x => x.InvoicedQuantity).HasColumnName("invoiced_quantity").IsRequired();
        builder.Property(x => x.PriceAmount).HasColumnName("price_amount").IsRequired(false);
        builder.Property(x => x.LineExtensionAmount).HasColumnName("line_extension_amount").IsRequired(false);
        builder.Property(x => x.Percent).HasColumnName("percent").IsRequired(false);
        builder.Property(x => x.TaxAmount).HasColumnName("tax_amount").IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnName("unit_price").IsRequired(false);
        builder.Property(x => x.Invoice).HasColumnName("invoice").IsRequired(false);
        builder.Property(x => x.DateRegister).HasColumnName("dateregister").IsRequired(false);
        builder.Property(x => x.Verify).HasColumnName("verify").IsRequired(false);
    }	
}
