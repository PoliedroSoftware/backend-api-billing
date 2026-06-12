using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.PedingInvoice.Entities;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Entities;
using Poliedro.Billing.Domain.Provider.Entities;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

public class DataBaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ServerEntity> Server { get; set; }
    public DbSet<DianResolutionEntity> DianResolution { get; set; }
    public DbSet<CompanyProviderEntity> CompanyProvider { get; set; }
    public DbSet<DianResolutionEntity> DianResolutionCreditNote { get; set; }
    public DbSet<ClientEntity> ClientBillingElectronic { get; set; }
    public DbSet<InvoiceEntity> Invoices { get; set; }
    public DbSet<PedingInvoiceEntity> PedingInvoice { get; set; }
    public DbSet<InvoiceElectronic> InvoiceElectronics { get; set; }
    public DbSet<ProviderEntity> ProviderEntities { get; set; }
    public DbSet<InvoiceWithDetailElectronic> InvoiceDetailElectronics { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        EntityConfiguration(modelBuilder);
    }

    private static void EntityConfiguration(ModelBuilder modelBuilder)
    {
        new ServerConfiguration(modelBuilder.Entity<ServerEntity>());
        new DianResolutionConfiguration(modelBuilder.Entity<DianResolutionEntity>());
        new CompanyProviderConfiguration(modelBuilder.Entity<CompanyProviderEntity>());
        new ClientBillingElectronicConfiguration(modelBuilder.Entity<ClientEntity>());
        new ProviderElectronicConfiguration(modelBuilder.Entity<ProviderEntity>());
    }
}
