using BookBoostApi.Interfaces;
using BookBoostApi.Services;
using BookBoostApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<BookBoostDatabaseSettings>(
    builder.Configuration.GetSection("BookBoostDatabase")
);
builder.Services.Configure<RainforestSettings>(
    builder.Configuration.GetSection("RainforestSettings")
);
builder.Services.Configure<NotificationSettings>(
    builder.Configuration.GetSection("NotificationSettings")
);
builder.Services.Configure<OpenAiSettings>(
    builder.Configuration.GetSection("OpenAiSettings")
);
builder.Services.Configure<PaymentSettings>(
    builder.Configuration.GetSection("PaymentSettings")
);
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("AuthSettings")
);

builder.Services.AddTransient<IAmazonProductService, RainforestService>();
builder.Services.AddTransient<IEmailService, AmazonEmailService>();
builder.Services.AddTransient<ITemplateService, FluidService>();
builder.Services.AddTransient<IAiService, OpenAiService>();
builder.Services.AddTransient<IAppEmailService, AppEmailService>();
builder.Services.AddTransient<IPaymentService, StripeService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IAdService, AdService>();
builder.Services.AddSingleton<IPriceService, PriceService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            var uri = new Uri(origin);
            // Allow your Azure frontend and all localhost ports
            // "https://your-frontend.azurestaticapps.net"
            return uri.Host == "localhost" || origin == builder.Configuration["AzureHosting:FrontendUrl"];
        })
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // The key used to sign the token (same key you used to sign it in the first place)
        var key = Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:SecretKey"]);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate the issuer (should match the issuer in your JWT)
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AuthSettings:Audience"], // Example: api://my-custom-api

            // Validate the audience (should match the audience in your JWT)
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AuthSettings:Audience"], // Example: api://my-custom-api

            // Validate the token expiration (exp claim)
            ValidateLifetime = true,

            // Set the key used to validate the JWT
            IssuerSigningKey = new SymmetricSecurityKey(key),
            
            RoleClaimType = ClaimTypes.Role 
        };

        options.SaveToken = true; // Save the JWT token in the HTTP context
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiAccess", policy => policy.RequireAuthenticatedUser());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
