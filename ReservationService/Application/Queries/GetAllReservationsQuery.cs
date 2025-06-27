using MediatR;
using ReservationService.Domain.Entities;

namespace ReservationService.Application.Queries;

public class GetAllReservationsQuery : IRequest<List<ReservationEntity>> {}