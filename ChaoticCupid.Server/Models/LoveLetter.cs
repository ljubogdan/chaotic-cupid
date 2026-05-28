namespace ChaoticCupid.Server.Models;

public class LoveLetter
{
    public string FromUsername { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public int FromYear { get; set; }
    public string FromPhone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
