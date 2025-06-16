using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.FERetail.Ports;

namespace WorkerServiceBilling
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(
            ILogger<Worker> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    using (var scope = _serviceProvider.CreateScope()) 
                    {
                        var clientDomainService = scope.ServiceProvider.GetRequiredService<IClientDomainService>();
                        var clients = await clientDomainService.GetAllAsync(cancellationToken);
                        var billingService = scope.ServiceProvider.GetRequiredService<IBillingService>();
                        var retailService = scope.ServiceProvider.GetRequiredService<IFERetailService>();

                        if (clients.Value is not null)
                        {
                            var retail = await retailService.CreateElectronicInvoicesAsync(clients.Value!, cancellationToken);
                            var eds = await billingService.CreateInvoicesPosAsync(clients.Value!, cancellationToken);

                        }

                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    }
                }

                await Task.Delay(90000, cancellationToken); 
            }
        }
    }
}
