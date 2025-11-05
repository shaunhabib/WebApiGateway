using ApiGateway;
using PEPSignal;
using System.Text.Json;
using ApiGateway.Abstraction;
using ApiGateway.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var router = app.Services.GetRequiredService<SignalRRouterService>();
    router.StartListening();
});

app.Run();

