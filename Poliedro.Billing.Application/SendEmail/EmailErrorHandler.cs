namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendEmail;
public class EmailErrorHandler
{
    public void Handle(Exception ex)
    {
        Console.WriteLine($"[Error Email] {ex.GetType().Name}: {ex.Message}");
    }
}
