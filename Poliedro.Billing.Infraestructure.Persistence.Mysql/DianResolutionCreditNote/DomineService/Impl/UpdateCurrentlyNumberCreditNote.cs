using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolutionCreditNote.DomineService.Impl;

public class UpdateCurrentlyNumberCreditNote(
    IDianResolutionDomainService dianResolutionDomainService,
    IConfiguration configuration
    ) : IUpdateCurrentlyNumberCreditNote
{
    public async Task UpdateCurrentlyNumberCreditNoteAsync(
        ParametersCurrentlyNumber Parameters,
        CancellationToken cancellationToken)
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

                var Sql = @"UPDATE dian_resolution_credit_note SET currently_number = @NewNumber, currently_date = @NewDate, lasted_invoiced = @LastedInvoiced WHERE resolutionid = @ResolutionId";
                await using var command = new MySqlCommand(Sql, Connection);
                command.Parameters.AddWithValue("@NewNumber", Parameters.Invoice);
                command.Parameters.AddWithValue("@NewDate", DateTime.Now);
                command.Parameters.AddWithValue("@LastedInvoiced", Parameters.LastedInvoiced);
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

                var Sql = @"UPDATE dian_resolution_credit_note SET expirated = 1 WHERE resolutionid = @ResolutionId";
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
