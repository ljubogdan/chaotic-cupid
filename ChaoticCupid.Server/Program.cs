using ChaoticCupid.Server.Hubs;
using ChaoticCupid.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PersonRegistry>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<CupidService>();

var app = builder.Build();

app.MapHub<CupidHub>("/cupid");

app.Run();
