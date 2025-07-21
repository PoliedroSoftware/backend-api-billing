using Poliedro.Billing.Domain.Billing.Ports;
namespace Poliedro.Billing.Domain.Common.Methods.Billing.Validate;
public class BillingValidateNumberDate : IBillingValidateRangeExpiration
{
    public Task ValidateNumerationDateAsync(int Invoice,DateTime ExpirationDate, int FinalRange)
    {
        bool expirated = Invoice > FinalRange || DateTime.Now > ExpirationDate;


        if (expirated)
        {
            //send Email vencimiento de la numeración o fecha


            //await updateCurrentlyNumber.UpdateCurrentlyNumberAsync(
            //   new ParametersCurrentlyNumber(invoice, date.ToString(), clientItem.ResolutionId),
            //   cancellationToken);


        }

        throw new NotImplementedException();
    }
}
