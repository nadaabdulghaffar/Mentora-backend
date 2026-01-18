using FluentValidation;
using Mentora.Application.DTOs.Auth;
using Mentora.Domain.Enums;
namespace Mentora.Application.Validators;

public class CompleteMenteeProfileRequestValidator : AbstractValidator<CompleteMenteeProfileRequest>
{
    public CompleteMenteeProfileRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.DomainId)
            .GreaterThan(0).WithMessage("Domain must be selected");

        RuleFor(x => x.EducationStatus)
            .NotEmpty().WithMessage("Education status is required")
            .Must(BeValidEducationStatus).WithMessage("Invalid education status");

        RuleFor(x => x.ExperienceLevel)
            .NotEmpty().WithMessage("Experience level is required")
            .Must(BeValidExperienceLevel).WithMessage("Invalid experience level");

        RuleFor(x => x.TechnologyIds)
            .NotEmpty().WithMessage("At least one technology must be selected")
            .Must(x => x.Count <= 10).WithMessage("Cannot select more than 10 technologies");

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters");
    }

    private bool BeValidEducationStatus(string status)
    {
        return Enum.TryParse<Domain.Enums.EducationStatus>(status, true, out _);
    }

    private bool BeValidExperienceLevel(string level)
    {
        return Enum.TryParse<Domain.Enums.ExperienceLevel>(level, true, out _);
    }
}