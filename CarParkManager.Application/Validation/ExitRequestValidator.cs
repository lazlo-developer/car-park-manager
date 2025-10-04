using FluentValidation;
using CarParkManager.Application.Models;

namespace CarParkManager.Application.Validation;

public class ExitRequestValidator : AbstractValidator<ExitRequest>
{
    public ExitRequestValidator()
    {
        RuleFor(x => x.VehicleReg)
            .NotEmpty()
            .MaximumLength(16);
    }
}
