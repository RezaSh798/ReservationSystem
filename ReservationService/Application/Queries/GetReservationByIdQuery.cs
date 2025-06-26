using MediatR;
using Reservation.Domain.Entities;

namespace Reservation.Application.Queries;

public class GetReservationByIdQuery : IRequest<ReservationEntity>
{
    public Guid Id { get; set; }
}