namespace BookBoostApi.Interfaces;

public interface IEmailService
{
    Task<bool> EmailAsync(string recipientEmail, string subject, string body);
}