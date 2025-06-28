using System.Text;
using System.Text.Json;
using Notification.Domain.Entity;
using Notification.Infrastructure.Persistence;
using NotificationService.Application.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reservation.Contracts.Events;

namespace NotificationService.Infrastructure.Messaging;

public class ReservationNotificationConsumer
    (IEmailService emailService, IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ReservationNotificationConsumer> logger)
    : BackgroundService
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly IEmailService _emailService = emailService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = logger;

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
        await _channel.BasicConsumeAsync(queue: "reservation-removed", autoAck: true, consumer: consumer);

        return;
    }

    private async Task SendConfirmEmailAsync(string message)
    {
        ReservationCreatedEvent? evt = JsonSerializer.Deserialize<ReservationCreatedEvent>(message);
        _logger.LogInformation($"Reservation created with id {evt?.ReservationId} was received :)");

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
        ReservationRemovedEvent? evt = JsonSerializer.Deserialize<ReservationRemovedEvent>(message);
        _logger.LogInformation($"Reservation removed with id {evt?.ReservationId} was received :)");

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
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: "reservation-created", durable: true, exclusive: false, autoDelete: false, arguments: null);
        await _channel.QueueDeclareAsync(queue: "reservation-removed", durable: true, exclusive: false, autoDelete: false, arguments: null);
    }
}