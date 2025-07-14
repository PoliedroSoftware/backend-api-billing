namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICalculateCheckDigits
{
    Task<string> CalculateCheckDigit(string IdentificationNumber, CancellationToken cancellationToken);
}
