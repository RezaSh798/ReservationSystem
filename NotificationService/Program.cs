using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Messaging;
using Notification.Infrastructure.Persistence;
using NotificationService.Infrastructure.Services;
using NotificationService.Application.Services;
using Notification.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDatabase")));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHostedService<ReservationNotificationConsumer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();