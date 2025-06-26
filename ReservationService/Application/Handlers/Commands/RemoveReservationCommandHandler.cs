using System.Text;
using System.Text.Json;
using MediatR;
using RabbitMQ.Client;
using Reservation.Application.Commands;
using Reservation.Domain.Entities;
using Reservation.Infrastructure.Persistence;

namespace Reservation.Application.Handlers.Commands;

public class RemoveReservationCommandHandler
    (ReservationDbContext context)
    : IRequestHandler<RemoveReservationCommand, Guid>
{
    private readonly ReservationDbContext _context = context;

    public async Task<Guid> Handle(RemoveReservationCommand request, CancellationToken cancellationToken)
    {
        ReservationEntity? reservation = await _context.Reservations.FindAsync(request.Id);
        if (reservation == null) return null;

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "reservation-removed",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        var json = JsonSerializer.Serialize(new ReservationRemovedEvent
        {
            GetReservationByIdQuery = request.Id,
            Email = reservation.Email
        });
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublishAsync(
            exchange: "",
            routingKey: "reservation-removed",
            basicProperties: null,
            body: body
        );

        return request.Id;
    }
}