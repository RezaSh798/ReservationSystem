using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReservationService.Application.Commands;
using ReservationService.Application.Queries;
using ReservationService.Domain.Entities;

namespace ReservationService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReservationsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<ReservationEntity>>> GetAll()
    {
        var reservations = await _mediator.Send(new GetAllReservationsQuery());
        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationEntity>> GetById(Guid id)
    {
        var reservation = await _mediator.Send(new GetReservationByIdQuery { Id = id });
        if (reservation == null) return NotFound();
        return Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<ReservationEntity>> Create(CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReservationEntity>> Update(Guid id, UpdateReservationCommand command)
    {
        if (id != command.Id) return BadRequest();
        var updated = await _mediator.Send(command);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new RemoveReservationCommand { Id = id });
        return NoContent();
    }
}