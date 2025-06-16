namespace Poliedro.Billing.Application.Helper.DigitalVerification;

public static class DigitalVerification
{
    public static string CalcularDigitoVerificacion(string numero)
    {
        if (string.IsNullOrEmpty(numero) || !long.TryParse(numero, out _))
        {
            return "error";
        }

        int[] pesos = { 3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71 };
        int longitud = numero.Length;
        int suma = 0;

        for (int i = 0; i < longitud; i++)
        {
            int digito = int.Parse(numero[longitud - i - 1].ToString());
            suma += digito * pesos[i % pesos.Length];
        }

        int residuo = suma % 11;
        int digitoVerificacion = residuo > 1 ? 11 - residuo : residuo;

        return digitoVerificacion.ToString();
    }
}
