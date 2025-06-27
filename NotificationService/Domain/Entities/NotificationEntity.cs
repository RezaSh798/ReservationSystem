namespace Notification.Domain.Entity;

public class NotificationEntity
{
    public int Id { get; set; }
    public Guid ReservationId { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}