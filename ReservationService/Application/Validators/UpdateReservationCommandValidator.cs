using FluentValidation;
using Reservation.Application.Commands;

namespace Reservation.Application.Validators;

public class UpdateReservationCommandValidator : AbstractValidator<UpdateReservationCommand>
{
    public UpdateReservationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("شناسه رزرو الزامی است.");
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.ReservationDate).GreaterThan(DateTime.Now);
    }
}