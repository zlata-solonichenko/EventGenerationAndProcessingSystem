using EventGenerationAndProcessingSystem;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Получение строки подключения из конфигурации
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Регистрация DbContext с использованием Npgsql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

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