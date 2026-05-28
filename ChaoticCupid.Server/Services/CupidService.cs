using ChaoticCupid.Server.Interfaces;
using ChaoticCupid.Server.Models;
using Microsoft.AspNetCore.SignalR;
using ChaoticCupid.Server.Hubs;
using System.Security.Cryptography;

namespace ChaoticCupid.Server.Services;

public class CupidService : BackgroundService, ICupidInterface
{
    private static readonly string[] Messages =
    [
        "Radujem se našem susretu!",
        "Želim da se upoznamo.",
        "Nisam zainteresovan/a za upoznavanje."
    ];

    private readonly PersonRegistry _registry;
    private readonly IHubContext<CupidHub> _hubContext;
    private readonly ILogger<CupidService> _logger;

    public CupidService(PersonRegistry registry, IHubContext<CupidHub> hubContext, ILogger<CupidService> logger)
    {
        _registry = registry;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            await SendLettersToAll();
        }
    }

    public async Task SendLettersToAll()
    {
        var all = _registry.GetAll();
        if (all.Count < 2) return;

        foreach (var recipient in all)
        {
            if (recipient.HasPendingLetter) continue;

            var candidates = all.Where(p =>
                !p.Username.Equals(recipient.Username, StringComparison.OrdinalIgnoreCase) &&
                !recipient.BlockedUsers.Contains(p.Username));

            var (match, _) = FindBestMatch(recipient, candidates);
            if (match == null) continue;

            recipient.HasPendingLetter = true;

            var message = Messages[RandomNumberGenerator.GetInt32(Messages.Length)];
            var letter = new LoveLetter
            {
                FromUsername = match.Username,
                FromCity = match.City,
                FromYear = match.Year,
                FromPhone = message == Messages[2] ? string.Empty : match.Phone,
                Message = message
            };

            await _hubContext.Clients.Client(recipient.ConnectionId)
                .SendAsync("ReceiveLetter", letter);

            _logger.LogInformation("Cupid sent letter to {Recipient} from {Sender}", recipient.Username, match.Username);
        }
    }

    public (Person? match, int score) FindBestMatch(Person recipient, IEnumerable<Person> candidates)
    {
        Person? best = null;
        int bestScore = -1;

        foreach (var candidate in candidates)
        {
            int score = 0;

            if (string.Equals(recipient.City, candidate.City, StringComparison.OrdinalIgnoreCase))
                score += 30;

            if (Math.Abs(recipient.Year - candidate.Year) <= 2)
                score += 20;

            score += RandomNumberGenerator.GetInt32(101);

            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return (best, bestScore);
    }
}
