using FluentValidation;
using Mentora.Application.DTOs.Auth;
using Mentora.Domain.Enums;

namespace Mentora.Application.Validators;

public class CompleteMentorProfileRequestValidator : AbstractValidator<CompleteMentorProfileRequest>
{
    public CompleteMentorProfileRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.DomainId)
            .GreaterThan(0).WithMessage("Domain must be selected");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThan(0).WithMessage("Years of experience must be greater than 0")
            .LessThanOrEqualTo(50).WithMessage("Years of experience seems invalid");

        RuleFor(x => x.LinkedInUrl)
            .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("Invalid LinkedIn URL");

        RuleFor(x => x.TechnologyIds)
            .NotEmpty().WithMessage("At least one technology expertise must be selected")
            .Must(x => x.Count <= 15).WithMessage("Cannot select more than 15 technologies");

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters");
    }

    private bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}