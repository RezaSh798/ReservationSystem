namespace ReservationService.Domain.Entities;

public class ReservationEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime ReservationDate { get; set; }
    private ReservationEntity() { }
    public ReservationEntity(string name, string email, DateTime date)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        ReservationDate = date;
    }
}