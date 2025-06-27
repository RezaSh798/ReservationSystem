namespace Reservation.Contracts.Events;

public class ReservationCreatedEvent
{
    public Guid ReservationId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime ReservationDate { get; set; }
    public string ServiceName { get; set; }
    public ReservationCreatedEvent() { }

    public ReservationCreatedEvent(Guid reservationId, string name, string email, DateTime reservationDate, string serviceName)
    {
        ReservationId = reservationId;
        Name = name;
        Email = email;
        ReservationDate = reservationDate;
        ServiceName = serviceName;
    }
}