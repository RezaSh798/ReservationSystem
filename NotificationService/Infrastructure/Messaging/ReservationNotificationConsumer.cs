using System.Text;
using System.Text.Json;
using Notification.Domain.Entity;
using Notification.Infrastructure.Persistence;
using NotificationService.Application.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notification.Infrastructure.Messaging;

public class ReservationNotificationConsumer
    (IEmailService emailService, IServiceProvider serviceProvider, IConfiguration configuration)
    : BackgroundService
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly IEmailService _emailService = emailService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IConfiguration _configuration = configuration;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitChannel();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            if (eventArgs.RoutingKey == "reservation-created")
                await SendConfirmEmailAsync(message);
            else
                await SendCancellationEmailAsync(message);
        };

        await _channel.BasicConsumeAsync(queue: "reservation-created", autoAck: true, consumer: consumer);
        await _channel.BasicConsumeAsync(queue: "reservation-deleted", autoAck: true, consumer: consumer);

        return;
    }

    private async Task SendConfirmEmailAsync(string message)
    {
        var evt = JsonSerializer.Deserialize<ReservationCreatedEvent>(message);

        var subject = "Reservation Confirmed";
        var html = $"<p>رزرو شما با شماره {evt.ReservationId} با موفقیت ثبت شد.</p>";
        await _emailService.SendAsync(evt.Email, subject, html);

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        db.Notifications.Add(new NotificationEntity
        {
            ReservationId = evt.ReservationId,
            Email = evt.Email,
            Message = "رزرو شما با موفقیت ثبت شد.",
            CreatedAt = DateTime.Now
        });
        await db.SaveChangesAsync();
    }

    private async Task SendCancellationEmailAsync(string message)
    {
        var evt = JsonSerializer.Deserialize<ReservationRemovedEvent>(message);

        var subject = "Reservation Canceled";
        var html = $"<p>رزرو شما با شماره {evt.ReservationId} لغو شد.</p>";
        await _emailService.SendAsync(evt.Email, subject, html);

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        db.Notifications.Add(new NotificationEntity
        {
            ReservationId = evt.ReservationId,
            Email = evt.Email,
            Message = "رزرو شما لغو شد.",
            CreatedAt = DateTime.Now
        });
        await db.SaveChangesAsync();
    }

    public override async void Dispose()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
        base.Dispose();
    }

    private async Task InitChannel()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: "reservation-created", durable: false, exclusive: false, autoDelete: false, arguments: null);
        await _channel.QueueDeclareAsync(queue: "reservation-removed", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }
}