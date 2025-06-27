using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entity;
using Notification.Infrastructure.Persistence;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NotificationsController(NotificationDbContext context) : ControllerBase
{
    private readonly NotificationDbContext _context;

    [HttpGet]
    public async Task<ActionResult<List<NotificationEntity>>> GetAll()
    {
        List<NotificationEntity> notifications = await _context.Notifications.ToListAsync();
        return Ok(notifications);
    }
}