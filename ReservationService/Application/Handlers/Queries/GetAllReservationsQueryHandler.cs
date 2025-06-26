using MediatR;
using Microsoft.EntityFrameworkCore;
using Reservation.Application.Queries;
using Reservation.Domain.Entities;
using Reservation.Infrastructure.Persistence;

namespace Reservation.Application.Handlers.Queries;

public class GetAllReservationsQueryHandler
    (ReservationDbContext context)
    : IRequestHandler<GetAllReservationsQuery, List<ReservationEntity>>
{
    private readonly ReservationDbContext _context = context;

    public async Task<List<ReservationEntity>> Handle(GetAllReservationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Reservations.ToListAsync(cancellationToken);
    }
}