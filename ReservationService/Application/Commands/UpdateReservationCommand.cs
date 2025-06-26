using MediatR;
using Reservation.Domain.Entities;

namespace Reservation.Application.Commands;

public class UpdateReservationCommand : IRequest<ReservationEntity>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime ReservationDate { get; set; }
}