using MediatR;
using Reservation.Domain.Entities;

namespace Reservation.Application.Commands;

class CreateReservationCommand : IRequest<ReservationEntity>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime ReservationDate { get; set; }
}