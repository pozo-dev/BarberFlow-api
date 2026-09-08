using BarberFlow.Domain.Interfaces.Services;

namespace BarberFlow.Infrastructure.Services;
public class FakeSmsService : ISmsService
{
    public Task SendAsync(string phoneNumber, string message)
    {
        Console.WriteLine($"[SMS SIMULADO] → {phoneNumber}: {message}");
        return Task.CompletedTask;
    }
}
