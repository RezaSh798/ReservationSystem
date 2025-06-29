using System.Text;
using System.Text.Json;
using MediatR;
using RabbitMQ.Client;
using ReservationService.Application.Commands;
using ReservationService.Domain.Entities;
using ReservationService.Infrastructure.Persistence;
using Reservation.Contracts.Events;

namespace ReservationService.Application.Handlers.Commands;

public class CreateReservationCommandHandler
    (ReservationDbContext context, ILogger<CreateReservationCommandHandler> logger)
    : IRequestHandler<CreateReservationCommand, ReservationEntity>
{
    private readonly ReservationDbContext _context = context;
    private readonly ILogger<CreateReservationCommandHandler> _logger = logger;

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
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        var json = JsonSerializer.Serialize(new ReservationCreatedEvent
        {
            ReservationId = reservation.Id,
            Name = reservation.Name,
            Email = reservation.Email,
            ReservationDate = reservation.ReservationDate
        });

        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: "reservation-created",
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: body
        );

        _logger.LogInformation($"Reservation with id {reservation.Id} was created ;)");
        return reservation;
    }
}