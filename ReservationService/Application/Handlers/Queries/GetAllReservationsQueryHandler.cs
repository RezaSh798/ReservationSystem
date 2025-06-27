using MediatR;
using Microsoft.EntityFrameworkCore;
using ReservationService.Application.Queries;
using ReservationService.Domain.Entities;
using ReservationService.Infrastructure.Persistence;

namespace ReservationService.Application.Handlers.Queries;

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