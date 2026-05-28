namespace ChaoticCupid.Server.Models;

public class Person
{
    public string Username { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public bool HasPendingLetter { get; set; } = false;
    public HashSet<string> BlockedUsers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
