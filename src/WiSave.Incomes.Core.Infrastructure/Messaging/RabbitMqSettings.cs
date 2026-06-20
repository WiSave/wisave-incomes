using Microsoft.Extensions.Configuration;

namespace WiSave.Incomes.Core.Infrastructure.Messaging;

public sealed record RabbitMqSettings(
    string Host,
    string VirtualHost,
    string Username,
    string Password,
    int Port)
{
    public static RabbitMqSettings FromConfiguration(IConfiguration configuration)
    {
        return new RabbitMqSettings(
            Host: configuration["RabbitMq:Host"] ?? "localhost",
            VirtualHost: configuration["RabbitMq:VirtualHost"] ?? "incomes",
            Username: configuration["RabbitMq:Username"] ?? "guest",
            Password: configuration["RabbitMq:Password"] ?? "guest",
            Port: int.TryParse(configuration["RabbitMq:Port"], out var port) ? port : 5672);
    }
}
