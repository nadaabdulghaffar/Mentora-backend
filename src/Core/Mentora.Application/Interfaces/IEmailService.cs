namespace Mentora.Application.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string firstName, string verificationToken);
    Task SendWelcomeEmailAsync(string toEmail, string firstName, string role);
}