
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;

public class CalculateCheckDigitsBilling : ICalculateCheckDigits
{
    public Task<string> CalculateCheckDigit(string IdentificationNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(IdentificationNumber) || !long.TryParse(IdentificationNumber, out _))
        {
            return Task.FromResult("error");
        }

        int[] pesos = { 3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71 };
        int longitud = IdentificationNumber.Length;
        int suma = 0;

        for (int i = 0; i < longitud; i++)
        {
            int digito = int.Parse(IdentificationNumber[longitud - i - 1].ToString());
            suma += digito * pesos[i % pesos.Length];
        }

        int residuo = suma % 11;
        int digitoVerificacion = residuo > 1 ? 11 - residuo : residuo;

        return Task.FromResult(digitoVerificacion.ToString());
    }
}
