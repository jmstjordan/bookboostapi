

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class AppEmailService : IAppEmailService
{
    private IEmailService _emailService;
    private ITemplateService _templateService;
    private IUserService _userService;
    private IMongoCollection<NotificationTemplate> _templatesCollection;

    public AppEmailService(IEmailService emailService, ITemplateService templateService, IOptions<BookBoostDatabaseSettings> bookBoostDatabaseSettings, IUserService userService)
    {
        _emailService = emailService;
        _templateService = templateService;
        _userService = userService;

        var mongoClient = new MongoClient(
            bookBoostDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookBoostDatabaseSettings.Value.DatabaseName);

        _templatesCollection = mongoDatabase.GetCollection<NotificationTemplate>(
            bookBoostDatabaseSettings.Value.TemplatesCollectionName);
    }

    private async Task<bool> RenderAndSend(TemplateType templateType, dynamic model)
    {
        var templateDoc = await _templatesCollection.Find(x => x.Type == templateType).FirstOrDefaultAsync();
        string body = _templateService.RenderModel(model, templateDoc.Content);
        return await _emailService.EmailAsync("jmstjordan@gmail.com", templateDoc.Subject, body);
    }

    public async Task<bool> SendAdCreated(string userId)
    {
        // look up userId email
        return await RenderAndSend(TemplateType.AdCreated, new { Firstname = "Bill", Lastname = "Gates" });
    }

    public async Task<bool> SendAdAccepted(string userId)
    {
        // look up userId email
        return await RenderAndSend(TemplateType.AdAccepted, new { Firstname = "Bill", Lastname = "Gates" });
    }

    public async Task<bool> SendAdDeclined(string userId)
    {
        // look up userId email
        return await RenderAndSend(TemplateType.AdDeclined, new { Firstname = "Bill", Lastname = "Gates" });
    }

    public async Task<bool> SendUserCreated(User user)
    {
        // look up userId email
        // var user = await _userService.GetUser(userId);
        return await RenderAndSend(TemplateType.UserCreated, new { Firstname = "Bill", Lastname = "Gates" });
    }

    public async Task<bool> SendPromotionJoined(string userId)
    {
        // look up userId email
        return await RenderAndSend(TemplateType.PromotionJoined, new { Firstname = "Bill", Lastname = "Gates" });
    }
}