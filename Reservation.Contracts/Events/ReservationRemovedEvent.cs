namespace Reservation.Contracts.Events;

public class ReservationRemovedEvent
{
    public Guid ReservationId { get; set; }
    public string Email { get; set; }

    public ReservationRemovedEvent() { }

    public ReservationRemovedEvent(Guid reservationId, string email)
    {
        ReservationId = reservationId;
        Email = email;
    }
}
