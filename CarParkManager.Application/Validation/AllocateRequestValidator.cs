using FluentValidation;
using CarParkManager.Application.Models;

namespace CarParkManager.Application.Validation;

public class AllocateRequestValidator : AbstractValidator<AllocateRequest>
{
    public AllocateRequestValidator()
    {
        RuleFor(x => x.VehicleReg)
            .NotEmpty()
            .WithMessage("Please provide a vehicle registration number.")
            .MaximumLength(16);
            
        RuleFor(x => x.VehicleType)
            .IsInEnum();
    }
}
