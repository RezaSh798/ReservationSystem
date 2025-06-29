using System.Text;
using System.Text.Json;
using MediatR;
using RabbitMQ.Client;
using ReservationService.Application.Commands;
using ReservationService.Domain.Entities;
using ReservationService.Infrastructure.Persistence;
using Reservation.Contracts.Events;

namespace ReservationService.Application.Handlers.Commands;

public class RemoveReservationCommandHandler
    (ReservationDbContext context, ILogger<RemoveReservationCommandHandler> logger)
    : IRequestHandler<RemoveReservationCommand, Guid>
{
    private readonly ReservationDbContext _context = context;
    private readonly ILogger<RemoveReservationCommandHandler> _logger = logger;

    public async Task<Guid> Handle(RemoveReservationCommand request, CancellationToken cancellationToken)
    {
        ReservationEntity? reservation = await _context.Reservations.FindAsync(request.Id);
        // TODO: Need throw exception
        if (reservation == null) return request.Id;

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "reservation-removed",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        var json = JsonSerializer.Serialize(new ReservationRemovedEvent
        {
            ReservationId = request.Id,
            Email = reservation.Email
        });
        var body = Encoding.UTF8.GetBytes(json);
        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: "reservation-removed",
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: body
        );

        _logger.LogInformation($"Reservation with id {reservation.Id} was removed ;)");
        return request.Id;
    }
}