using BookBoostApi.Interfaces;
using BookBoostApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.Configure<BookBoostDatabaseSettings>(
    builder.Configuration.GetSection("BookBoostDatabase")
);

builder.Services.Configure<ApiKeySettings>(
    builder.Configuration.GetSection("ApiKeys")
);

builder.Services.AddTransient<IAmazonProductService, RainforestService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IAdService, AdService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
