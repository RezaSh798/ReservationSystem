using MediatR;
using ReservationService.Domain.Entities;

namespace ReservationService.Application.Commands;

public class CreateReservationCommand : IRequest<ReservationEntity>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime ReservationDate { get; set; }
}