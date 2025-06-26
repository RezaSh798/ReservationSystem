using MediatR;
using Reservation.Application.Commands;
using Reservation.Domain.Entities;
using Reservation.Infrastructure.Persistence;

public class UpdateReservationCommandHandler
    (ReservationDbContext context)
    : IRequestHandler<UpdateReservationCommand, ReservationEntity>
{
    private readonly ReservationDbContext _context = context;

    public async Task<ReservationEntity> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
    {
        ReservationEntity? reservation = await _context.Reservations.FindAsync(request.Id);
        if (reservation == null) return null;

        _context.Entry(reservation).CurrentValues.SetValues(request);
        await _context.SaveChangesAsync();

        return reservation;
    }
}