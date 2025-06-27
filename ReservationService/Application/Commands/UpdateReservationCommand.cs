using MediatR;
using ReservationService.Domain.Entities;

namespace ReservationService.Application.Commands;

public class UpdateReservationCommand : IRequest<ReservationEntity>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime ReservationDate { get; set; }
}