namespace Poliedro.Billing.Domain.FERetail.Ports;

public interface ISendMessage
{
    Task SendMessageAsync(string Invoice, string Customer);
}
