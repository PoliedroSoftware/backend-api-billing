using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.UpdateCurrentlyNumber;

public class UpdateCurrentlyNumberService(
    IDianResolutionDomainService dianResolutionDomainService,
    IConfiguration configuration,
    IRelationalConnectionFactory connectionFactory) : IUpdateCurrentlyNumber
{
    public async Task UpdateCurrentlyNumberAsync(ParametersCurrentlyNumber parameters, CancellationToken cancellationToken)
    {
        try
        {
            var resolutionResult = await dianResolutionDomainService.GetByIdAsync(parameters.ResolutionId, cancellationToken);
            if (resolutionResult?.Value is null)
            {
                Console.WriteLine($"Resolution with ID {parameters.ResolutionId} not found.");
                return;
            }

            var currentNumber = resolutionResult.Value.CurrentRange;
            if (currentNumber < parameters.Invoice)
            {
                await using var connection = connectionFactory.Create(configuration.GetConnectionString("MysqlConnection")!);
                await connection.OpenAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = "UPDATE dian_resolution_billing SET current_number = @NewNumber, date_current = @NewDate WHERE resolution_id = @ResolutionId";
                AddParameter(command, "@NewNumber", parameters.Invoice);
                AddParameter(command, "@NewDate", (object?)parameters.CurrentlyDate ?? DBNull.Value);
                AddParameter(command, "@ResolutionId", parameters.ResolutionId);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            else
            {
                Console.WriteLine($"No update needed. Current current_number ({currentNumber}) is greater or equal to new invoice ({parameters.Invoice}).");
            }

            if (parameters.Expirated)
            {
                await using var connection = connectionFactory.Create(configuration.GetConnectionString("MysqlConnection")!);
                await connection.OpenAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = "UPDATE dian_resolution_billing SET expirated = 1 WHERE resolution_id = @ResolutionId";
                AddParameter(command, "@ResolutionId", parameters.ResolutionId);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Update Currently Number: " + ex.Message);
        }
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
