using MediatR;
using ReservationService.Application.Queries;
using ReservationService.Domain.Entities;
using ReservationService.Infrastructure.Persistence;

namespace ReservationService.Application.Handlers.Queries;

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