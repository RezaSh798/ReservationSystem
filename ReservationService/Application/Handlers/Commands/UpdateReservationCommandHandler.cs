using MediatR;
using ReservationService.Application.Commands;
using ReservationService.Domain.Entities;
using ReservationService.Infrastructure.Persistence;

public class UpdateReservationCommandHandler
    (ReservationDbContext context, ILogger<UpdateReservationCommandHandler> logger)
    : IRequestHandler<UpdateReservationCommand, ReservationEntity>
{
    private readonly ReservationDbContext _context = context;
    private readonly ILogger<UpdateReservationCommandHandler> _logger = logger;

    public async Task<ReservationEntity> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
    {
        ReservationEntity? reservation = await _context.Reservations.FindAsync(request.Id);
        if (reservation == null) return null;

        _context.Entry(reservation).CurrentValues.SetValues(request);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Reservation with id {reservation.Id} was updated ;)");
        return reservation;
    }
}