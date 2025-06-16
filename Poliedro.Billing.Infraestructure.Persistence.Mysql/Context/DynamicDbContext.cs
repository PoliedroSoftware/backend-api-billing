using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.EntityFramework.EntityConfigurations;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

public class DynamicDbContext(string _connectionString) : DbContext
{
    public DbSet<InvoiceEntity> Invoices { get; set; }
    public DbSet<InvoiceSuccessEntity> InvoiceSuccess { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InvoicesConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceSuccessEntityConfiguration());
    }
}
