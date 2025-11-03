using ApiGateway;
using PEPSignal;
using System.Text.Json;
using ApiGateway.Abstraction;
using ApiGateway.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalRHelper.RegisterHub(builder.Services);
builder.Services.AddGatewayService(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();
app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    // SignalRHelper.StartListening();
    // Console.WriteLine("Listening for SignalR messages...");
    var service = app.Services.GetRequiredService<Gateway>();
    service.StartListening();
});

app.Run();

