using MediatR;
using Reservation.Domain.Entities;

namespace Reservation.Application.Commands;

public class RemoveReservationCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}