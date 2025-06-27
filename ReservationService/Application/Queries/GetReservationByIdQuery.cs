using MediatR;
using ReservationService.Domain.Entities;

namespace ReservationService.Application.Queries;

public class GetReservationByIdQuery : IRequest<ReservationEntity>
{
    public Guid Id { get; set; }
}