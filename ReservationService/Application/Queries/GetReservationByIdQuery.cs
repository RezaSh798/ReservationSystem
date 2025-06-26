using MediatR;
using Reservation.Domain.Entities;

namespace Reservation.Application.Queries;

class GetReservationByIdQuery : IRequest<ReservationEntity>
{
    public Guid Id { get; set; }
}