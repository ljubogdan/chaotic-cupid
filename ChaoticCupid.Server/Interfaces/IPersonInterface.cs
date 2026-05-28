namespace ChaoticCupid.Server.Interfaces;

public interface IPersonInterface
{
    Task InitSinglePerson(string username, string city, int year, string phone);
    Task ConfirmLetter();
    Task BlockUser(string username);
}
