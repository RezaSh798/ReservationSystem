using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ReservationService.Infrastructure.Messaging;
using ReservationService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ReservationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReservationDatabase")));

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.Run();