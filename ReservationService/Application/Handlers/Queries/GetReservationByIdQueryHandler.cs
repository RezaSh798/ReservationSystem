using MediatR;
using Reservation.Application.Queries;
using Reservation.Domain.Entities;
using Reservation.Infrastructure.Persistence;

namespace Reservation.Application.Handlers.Queries;

public class GetReservationByIdQueryHandler
    (ReservationDbContext context)
    : IRequestHandler<GetReservationByIdQuery, ReservationEntity>
{
    private readonly ReservationDbContext _context = context;

    public async Task<ReservationEntity> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Reservations.FindAsync(request.Id);
    }
}