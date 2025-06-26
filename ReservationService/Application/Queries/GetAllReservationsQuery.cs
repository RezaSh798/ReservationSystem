using MediatR;
using Reservation.Domain.Entities;

namespace Reservation.Application.Queries;

public class GetAllReservationsQuery : IRequest<List<ReservationEntity>> {}