using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;

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
            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        using (var scope = _serviceProvider.CreateScope()) 
            //        {
            //            var clientDomainService = scope.ServiceProvider.GetRequiredService<IClientDomainService>();
            //            var clients = await clientDomainService.GetAllAsync(cancellationToken);
            //            var billingService = scope.ServiceProvider.GetRequiredService<IBillingService>();
                      
            //            if (clients.Value is not null)
            //            {
                           

            //            }

            //            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //        }
            //    }

            //    await Task.Delay(90000, cancellationToken); 
            //}
        }
    }
}
