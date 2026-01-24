using FluentValidation;
using Mentora.Application.DTOs.Auth;

namespace Mentora.Application.Validators;

public class CompleteMentorProfileRequestValidator
    : AbstractValidator<CompleteMentorProfileRequest>
{
    public CompleteMentorProfileRequestValidator()
    {
        RuleFor(x => x.DomainId)
            .GreaterThan(0)
            .WithMessage("Career field must be selected");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThan(0)
            .WithMessage("Years of experience must be greater than 0")
            .LessThanOrEqualTo(50)
            .WithMessage("Years of experience seems invalid");

        RuleFor(x => x.LinkedInUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("Invalid LinkedIn URL");

        RuleFor(x => x.SubDomainIds)
            .NotEmpty()
            .WithMessage("At least one area of expertise must be selected");

        RuleFor(x => x.TechnologyIds)
            .NotEmpty()
            .WithMessage("At least one tool must be selected")
            .Must(x => x.Count >= 1 && x.Count <= 5)
            .WithMessage("Please select between 1 and 5 tools");

        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .WithMessage("Bio cannot exceed 1000 characters");

        RuleFor(x => x.CountryCode)
            .Length(2)
            .When(x => !string.IsNullOrEmpty(x.CountryCode))
            .WithMessage("Invalid country code");
    }

    private bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp
                || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
