using MediatR;
using Reservation.Domain.Entities;

namespace Reservation.Application.Commands;

public class RemoveReservationCommand : IRequest<ReservationEntity>
{
    public Guid Id { get; set; }
}