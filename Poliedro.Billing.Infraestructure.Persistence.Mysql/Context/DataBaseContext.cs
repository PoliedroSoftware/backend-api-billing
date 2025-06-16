using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.PedingInvoice.Entities;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

public class DataBaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ServerEntity> Server { get; set; }
    public DbSet<DianResolutionEntity> DianResolution { get; set; }
    public DbSet<ClientEntity> ClientBillingElectronic { get; set; }
    public DbSet<InvoiceEntity> Invoices { get; set; }
    public DbSet<PedingInvoiceEntity> PedingInvoice { get; set; }

    public DbSet<InvoiceElectronic> InvoiceElectronics { get; set; }

    public DbSet<Domain.InvoiceDetailElectronic.Entities.InvoiceWithDetailElectronic> InvoiceDetailElectronics { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        EntityConfiguration(modelBuilder);
    }

    private static void EntityConfiguration(ModelBuilder modelBuilder)
    {
        new ServerConfiguration(modelBuilder.Entity<ServerEntity>());
        new DianResolutionConfiguration(modelBuilder.Entity<DianResolutionEntity>());
        new ClientBillingElectronicConfiguration(modelBuilder.Entity<ClientEntity>());
        new InvoiceElectronicConfiguration(modelBuilder.Entity<InvoiceElectronic>());
        new InvoiceDetailElectronicConfiguration(modelBuilder.Entity<Domain.InvoiceDetailElectronic.Entities.InvoiceWithDetailElectronic>());
    }
}
