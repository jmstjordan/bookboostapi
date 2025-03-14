using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Options;
using BookBoostApi.Interfaces;
using BookBoostApi.Models;

public class AmazonEmailService : IEmailService
{
    private readonly AmazonSimpleEmailServiceClient _sesClient;
    private readonly string _senderEmail;

    public AmazonEmailService(IOptions<NotificationSettings> settings)
    {
        _sesClient = new AmazonSimpleEmailServiceClient(settings.Value.AwsKey, settings.Value.AwsSecret, RegionEndpoint.GetBySystemName(settings.Value.AwsRegion));
        _senderEmail = settings.Value.AwsSenderEmail;
    }

    public async Task<bool> EmailAsync(string recipientEmail, string subject, string body)
    {
        var sendRequest = new SendEmailRequest
        {
            Source = _senderEmail,
            Destination = new Destination
            {
                ToAddresses = new List<string> { recipientEmail }
            },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Html = new Content(body) // Use Text = new Content(body) for plain text emails
                }
            }
        };

        try
        {
            var response = await _sesClient.SendEmailAsync(sendRequest);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
            return false;
        }
    }
}
