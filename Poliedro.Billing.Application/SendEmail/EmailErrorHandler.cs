namespace Poliedro.Billing.Application.SendEmail;
public class EmailErrorHandler
{
    public void Handle(Exception ex)
    {
        Console.WriteLine($"[Error Email] {ex.GetType().Name}: {ex.Message}");
    }
}
