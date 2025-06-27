using MediatR;
using ReservationService.Domain.Entities;

namespace ReservationService.Application.Commands;

public class RemoveReservationCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}