using ChaoticCupid.Server.Models;

namespace ChaoticCupid.Server.Interfaces;

public interface ICupidInterface
{
    Task SendLettersToAll();
    (Person? match, int score) FindBestMatch(Person recipient, IEnumerable<Person> candidates);
}
