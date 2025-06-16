using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.UpdateCurrentlyNumber;
public class UpdateCurrentlyNumberService(
    IDianResolutionDomainService dianResolutionDomainService,
    IConfiguration configuration
    ) : IUpdateCurrentlyNumber
{
    public async Task UpdateCurrentlyNumberAsync(ParametersCurrentlyNumber Parameters, CancellationToken cancellationToken)
    {
        try
        {
            var resolutionResult = await dianResolutionDomainService.GetByIdAsync(Parameters.ResolutionId, cancellationToken);

            if (resolutionResult == null)
            {
                Console.WriteLine($"Resolution with ID {Parameters.ResolutionId} not found.");
                return;
            }

            var currentlyNumber = resolutionResult.Value.CurrentlyNumber;

            if (currentlyNumber < Parameters.Invoice)
            {
                await using var Connection = new MySqlConnection(configuration.GetConnectionString("MysqlConnection"));
                await Connection.OpenAsync(cancellationToken);

                var Sql = @"UPDATE dian_resolution SET currently_number = @NewNumber, currently_date = @NewDate WHERE resolutionid = @ResolutionId";
                await using var command = new MySqlCommand(Sql, Connection);
                command.Parameters.AddWithValue("@NewNumber", Parameters.Invoice);
                command.Parameters.AddWithValue("@NewDate", Parameters.CurrentlyDate);
                command.Parameters.AddWithValue("@ResolutionId", Parameters.ResolutionId);

                await command.ExecuteNonQueryAsync(cancellationToken);

                Console.WriteLine("Update Currently Number: " + Parameters.Invoice + "ResolutionID:" + Parameters.ResolutionId);
            }
            else
            {
                Console.WriteLine($"No update needed. Current currently_number ({currentlyNumber}) is greater or equal to new invoice ({Parameters.Invoice}).");
            }

            if (Parameters.Expirated)
            {
                await using var Connection = new MySqlConnection(configuration.GetConnectionString("MysqlConnection"));
                await Connection.OpenAsync(cancellationToken);

                var Sql = @"UPDATE dian_resolution SET expirated = 1 WHERE resolutionid = @ResolutionId";
                await using var command = new MySqlCommand(Sql, Connection);
                command.Parameters.AddWithValue("@ResolutionId", Parameters.ResolutionId);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
          
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Update Currently Number: " + ex.Message);
        }

    }
}