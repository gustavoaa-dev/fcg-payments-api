using FCG.PaymentsAPI.Application.Consumers;
using FCG.PaymentsAPI.Application.Services;
using FCG.PaymentsAPI.Domain.Interfaces;
using FCG.PaymentsAPI.Infrastructure.Data;
using FCG.PaymentsAPI.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);

builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Message<FCG.Shared.Events.OrderPlacedEvent>(m => m.SetEntityName("OrderPlacedEvent"));
        cfg.Message<FCG.Shared.Events.PaymentProcessedEvent>(m => m.SetEntityName("PaymentProcessedEvent"));

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();

// Metricas HTTP: registradas antes dos demais middlewares (mesmo padrao do users-api).
app.UseHttpMetrics();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<PaymentsDbContext>();
    db.Database.Migrate();
}

app.MapMetrics();

app.Run();

