using Microsoft.AspNetCore.SignalR.Client;

const string ServerUrl = "http://localhost:5000/cupid";

var connection = new HubConnectionBuilder()
    .WithUrl(ServerUrl)
    .WithAutomaticReconnect()
    .Build();

bool isRegistered = false;
bool hasPendingLetter = false;

connection.On<string>("Registered", msg =>
{
    Console.WriteLine($"\n[SERVER] {msg}");
    isRegistered = true;
});

connection.On<string>("Error", msg =>
{
    Console.WriteLine($"\n[GREŠKA] {msg}");
});

connection.On<string>("UserBlocked", msg =>
{
    Console.WriteLine($"\n[INFO] {msg}");
});

connection.On<string>("LetterConfirmed", _ =>
{
    hasPendingLetter = false;
    Console.WriteLine("[INFO] Možete primati nova pisma.");
});

connection.On<dynamic>("ReceiveLetter", letter =>
{
    hasPendingLetter = true;

    Console.WriteLine("\n╔══════════════════════════════════════╗");
    Console.WriteLine("║          💌 NOVO PISMO OD KUPIDONA    ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.WriteLine($"  Korisnik : {letter.fromUsername}");
    Console.WriteLine($"  Grad     : {letter.fromCity}");
    Console.WriteLine($"  Godište  : {letter.fromYear}");

    string phone = letter.fromPhone?.ToString() ?? string.Empty;
    if (!string.IsNullOrEmpty(phone))
        Console.WriteLine($"  Telefon  : {phone}");

    Console.WriteLine($"  Poruka   : \"{letter.message}\"");
    Console.WriteLine("────────────────────────────────────────");
    Console.WriteLine("Pritisnite ENTER da potvrdite prijem pisma.");
});

await connection.StartAsync();
Console.WriteLine("Povezani na server. Unesite podatke za prijavu.\n");

string username = ReadNonEmpty("Korisničko ime: ");
string city = ReadNonEmpty("Grad: ");
int year = ReadPositiveInt("Godina rođenja: ");
string phone = ReadNonEmpty("Broj telefona: ");

await connection.InvokeAsync("InitSinglePerson", username, city, year, phone);

Console.WriteLine("\nUputstvo: ukucajte /block <username> da blokirate korisnika, ili pritisnite ENTER za potvrdu pisma.\n");

while (true)
{
    string? input = Console.ReadLine();

    if (input == null) continue;

    if (input.StartsWith("/block ", StringComparison.OrdinalIgnoreCase))
    {
        string target = input[7..].Trim();
        if (string.IsNullOrEmpty(target))
        {
            Console.WriteLine("[GREŠKA] Unesite korisničko ime za blokiranje: /block <username>");
            continue;
        }
        await connection.InvokeAsync("BlockUser", target);
    }
    else if (input == string.Empty && hasPendingLetter)
    {
        await connection.InvokeAsync("ConfirmLetter");
    }
}

static string ReadNonEmpty(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? value = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(value))
            return value;
        Console.WriteLine("[GREŠKA] Polje ne sme biti prazno.");
    }
}

static int ReadPositiveInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? raw = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(raw))
        {
            Console.WriteLine("[GREŠKA] Polje ne sme biti prazno.");
            continue;
        }
        if (!int.TryParse(raw, out int value))
        {
            Console.WriteLine("[GREŠKA] Unesite broj, ne tekst.");
            continue;
        }
        if (value <= 0)
        {
            Console.WriteLine("[GREŠKA] Godina mora biti pozitivan broj.");
            continue;
        }
        return value;
    }
}
