using System.Text;
using System.Text.Json;
using MediatR;
using RabbitMQ.Client;
using Reservation.Application.Commands;
using Reservation.Domain.Entities;
using Reservation.Infrastructure.Persistence;

namespace Reservation.Application.Handlers.Commands;

public class CreateReservationCommandHandler(ReservationDbContext context) : IRequestHandler<CreateReservationCommand, ReservationEntity>
{
    private readonly ReservationDbContext _context = context;

    public async Task<ReservationEntity> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        ReservationEntity reservation = new(request.Name, request.Email, request.ReservationDate);
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: "reservation-created",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var json = JsonSerializer.Serialize(new ReservationCreateEvent
        {
            ReservationId = reservation.Id,
            Name = reservation.Name,
            Email = reservation.Email,
            ReservationDate = reservation.ReservationDate
        });

        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublishAsync(
            exchange: "",
            routingKey: "reservation-created",
            basicProperties: null,
            body: body
        );

        return reservation;
    }
}