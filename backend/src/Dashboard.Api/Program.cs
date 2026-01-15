using System.Text.Json.Serialization;
using Dashboard.Application;
using Dashboard.Application.Abstractions;
using Dashboard.Infrastructure;
using Dashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();
    await dbContext.Database.MigrateAsync();
    var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
    await DashboardSeedData.EnsureSeededAsync(dbContext, clock);
}

app.UseCors("Frontend");
app.UseAuthorization();

app.MapControllers();

app.Run();
