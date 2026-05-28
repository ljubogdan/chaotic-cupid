using ChaoticCupid.Server.Interfaces;
using ChaoticCupid.Server.Models;
using ChaoticCupid.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChaoticCupid.Server.Hubs;

public class CupidHub : Hub, IPersonInterface
{
    private readonly PersonRegistry _registry;
    private readonly ILogger<CupidHub> _logger;

    public CupidHub(PersonRegistry registry, ILogger<CupidHub> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    public async Task InitSinglePerson(string username, string city, int year, string phone)
    {
        var person = new Person
        {
            Username = username,
            City = city,
            Year = year,
            Phone = phone,
            ConnectionId = Context.ConnectionId
        };

        if (!_registry.TryRegister(person))
        {
            await Clients.Caller.SendAsync("Error", $"Korisnik '{username}' je već prijavljen.");
            return;
        }

        _logger.LogInformation("Registered: {Username} from {City}, born {Year}", username, city, year);
        await Clients.Caller.SendAsync("Registered", $"Uspešno ste prijavljeni, {username}!");
    }

    public async Task ConfirmLetter()
    {
        var person = _registry.GetByConnection(Context.ConnectionId);
        if (person == null) return;

        person.HasPendingLetter = false;
        await Clients.Caller.SendAsync("LetterConfirmed");
    }

    public async Task BlockUser(string username)
    {
        var person = _registry.GetByConnection(Context.ConnectionId);
        if (person == null) return;

        if (string.Equals(person.Username, username, StringComparison.OrdinalIgnoreCase))
        {
            await Clients.Caller.SendAsync("Error", "Ne možete blokirati sami sebe.");
            return;
        }

        person.BlockedUsers.Add(username);
        await Clients.Caller.SendAsync("UserBlocked", $"Korisnik '{username}' je blokiran.");
        _logger.LogInformation("{User} blocked {Blocked}", person.Username, username);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _registry.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
