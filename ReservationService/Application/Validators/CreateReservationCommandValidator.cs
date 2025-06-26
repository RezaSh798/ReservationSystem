using FluentValidation;
using Reservation.Application.Commands;

namespace Reservation.Application.Validators;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("نام کاربر الزامی است.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("ایمیل معتبر نیست.");
        RuleFor(x => x.ReservationDate).GreaterThan(DateTime.Now).WithMessage("تاریخ رزرو باید در آینده باشد.");
    }
}