using BookBoostApi.Interfaces;
using BookBoostApi.Services;
using BookBoostApi.Models;

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

builder.Services.AddTransient<IAmazonProductService, RainforestService>();
builder.Services.AddTransient<IEmailService, AmazonEmailService>();
builder.Services.AddTransient<ITemplateService, FluidService>();
builder.Services.AddTransient<IAiService, OpenAiService>();
builder.Services.AddTransient<IAppEmailService, AppEmailService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IAdService, AdService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
