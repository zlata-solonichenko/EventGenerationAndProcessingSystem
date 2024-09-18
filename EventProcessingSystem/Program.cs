using EventGenerationAndProcessingSystem;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Регистрация HttpClient
builder.Services.AddHttpClient();

// Регистрация DbContext с использованием Npgsql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BloggingDatabase")));

// Регистрация EventProcessorService
builder.Services.AddScoped<EventProcessorService>();
// Регистрация EventGeneratorService
builder.Services.AddScoped<EventGeneratorService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// dotnet ef migrations add InitialCreate --project EventProcessingSystem.csproj
app.UseHttpsRedirection();

app.UseAuthorization();

// app.UseRouting();
// app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.MapControllers();

app.Run();