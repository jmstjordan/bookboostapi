

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class AppEmailService : IAppEmailService
{
    private IEmailService _emailService;
    private ITemplateService _templateService;
    private IUserService _userService;
    private IProductService _productService;
    private IMongoCollection<NotificationTemplate> _templatesCollection;

    public AppEmailService(IEmailService emailService, ITemplateService templateService, IOptions<BookBoostDatabaseSettings> bookBoostDatabaseSettings, IUserService userService, IProductService productService)
    {
        _emailService = emailService;
        _templateService = templateService;
        _userService = userService;
        _productService = productService;

        var mongoClient = new MongoClient(
            bookBoostDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookBoostDatabaseSettings.Value.DatabaseName);

        _templatesCollection = mongoDatabase.GetCollection<NotificationTemplate>(
            bookBoostDatabaseSettings.Value.TemplatesCollectionName);
    }

    private async Task<bool> RenderAndSend(TemplateType templateType, dynamic model, string email)
    {
        var templateDoc = await _templatesCollection.Find(x => x.Type == templateType).FirstOrDefaultAsync();
        string body = _templateService.RenderModel(model, templateDoc.Content);
        return await _emailService.EmailAsync(email, templateDoc.Subject, body);
    }

    public async Task<bool> SendAdCreated(string userId, Ad ad)
    {
        var user = await _userService.GetUser(userId);
        var product = await _productService.GetProduct(ad.ProductId);
        return await RenderAndSend(TemplateType.AdCreated, new { Username = user.Username, Email = user.Email, OrderNumber = ad.OrderId, ProductId = product.ProductId, Title = product.Title, Date = ad.AdDate, Price = ad.Price, Image = product.Image }, user.Email);
    }

    public async Task<bool> SendAdAccepted(string userId, Ad ad)
    {
        var user = await _userService.GetUser(userId);
        var product = await _productService.GetProduct(ad.ProductId);
        return await RenderAndSend(TemplateType.AdAccepted, new { Username = user.Username, Email = user.Email, OrderNumber = ad.OrderId, ProductId = product.ProductId, Title = product.Title, Date = ad.AdDate, Price = ad.Price, Image = product.Image }, user.Email);
    }

    public async Task<bool> SendAdDeclined(string userId, Ad ad)
    {
        var user = await _userService.GetUser(userId);
        var product = await _productService.GetProduct(ad.ProductId);
        return await RenderAndSend(TemplateType.AdDeclined, new { Username = user.Username, Email = user.Email, OrderNumber = ad.OrderId, ProductId = product.ProductId, Title = product.Title, Date = ad.AdDate, Price = ad.Price, Image = product.Image }, user.Email);
    }

    public async Task<bool> SendAdCanceled(string userId, Ad ad)
    {
        var user = await _userService.GetUser(userId);
        var product = await _productService.GetProduct(ad.ProductId);
        return await RenderAndSend(TemplateType.AdCanceled, new { Username = user.Username, Email = user.Email, OrderNumber = ad.OrderId, ProductId = product.ProductId, Title = product.Title, Date = ad.AdDate, Price = ad.Price, Image = product.Image }, user.Email);
    }

    public async Task<bool> SendUserCreated(User user, Role role)
    {
        var template = role == Role.Author ? TemplateType.AuthorCreated : TemplateType.ReaderCreated;
        return await RenderAndSend(template, new { Username = user.Username, Email = user.Email }, user.Email);
    }

    public async Task<bool> SendPromotionJoined(string userId)
    {
        var user = await _userService.GetUser(userId);
        return await RenderAndSend(TemplateType.PromotionJoined, new { Firstname = "Bill", Lastname = "Gates" }, user.Email);
    }

    public async Task<bool> SendPasswordReset(User user, string resetUrl)
    {
        return await RenderAndSend(TemplateType.PasswordReset, new { Firstname = "Bill", Lastname = "Gates", ResetUrl = resetUrl }, user.Email);
    }
}