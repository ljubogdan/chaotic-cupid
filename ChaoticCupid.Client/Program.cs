using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

const string ServerUrl = "http://localhost:5104/cupid";

var connection = new HubConnectionBuilder()
    .WithUrl(ServerUrl)
    .Build();

bool hasPendingLetter = false;

AnsiConsole.Write(new FigletText("Haotični Kupidon").Color(Color.HotPink));
AnsiConsole.MarkupLine("[grey]Povezivanje na server...[/]");

connection.On<string>("Registered", msg =>
{
    AnsiConsole.MarkupLine($"\n[green]{Markup.Escape(msg)}[/]");
    AnsiConsole.MarkupLine("[grey]Pritisni [white]ENTER[/] za potvrdu pisma | [white]/block <username>[/] za blokiranje[/]\n");
});

connection.On<string>("Error", msg =>
{
    AnsiConsole.MarkupLine($"[red]{Markup.Escape(msg)}[/]");
});

connection.On<string>("UserBlocked", msg =>
{
    AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(msg)}[/]");
});

connection.On<string>("LetterConfirmed", _ =>
{
    hasPendingLetter = false;
    AnsiConsole.MarkupLine("[grey]Spremni ste za novo pismo.[/]");
});

connection.On<LoveLetter>("ReceiveLetter", letter =>
{
    hasPendingLetter = true;

    var phoneRow = string.IsNullOrEmpty(letter.FromPhone)
        ? string.Empty
        : $"\n  [grey]Telefon :[/]  [white]{Markup.Escape(letter.FromPhone)}[/]";

    var messageColor = letter.Message.StartsWith("Nisam") ? "red" : "green";

    var content = new Markup(
        $"  [grey]Od      :[/]  [hotpink bold]{Markup.Escape(letter.FromUsername)}[/]\n" +
        $"  [grey]Grad    :[/]  [white]{Markup.Escape(letter.FromCity)}[/]\n" +
        $"  [grey]Godište :[/]  [white]{letter.FromYear}[/]" +
        phoneRow +
        $"\n\n  [{messageColor}]\"{Markup.Escape(letter.Message)}\"[/]"
    );

    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Panel(content)
        .Header("[hotpink] Novo pismo od Kupidona [/]")
        .BorderColor(Color.HotPink)
        .Padding(1, 1));

    AnsiConsole.MarkupLine("[grey]Pritisni [white]ENTER[/] da potvrdiš prijem.[/]");
});

connection.Closed += _ =>
{
    AnsiConsole.MarkupLine("\n[red]Konekcija je prekinuta. Pokrenite ponovo aplikaciju i ulogujte se.[/]");
    return Task.CompletedTask;
};

while (true)
{
    try
    {
        await connection.StartAsync();
        break;
    }
    catch
    {
        AnsiConsole.MarkupLine("[red]Nema konekcije ka serveru. Pokušavam ponovo...[/]");
        await Task.Delay(3000);
    }
}

AnsiConsole.MarkupLine("[green]Povezan na server.[/]\n");

var username = ReadNonEmpty("Korisničko ime");
var city     = ReadNonEmpty("Grad");
var year     = ReadPositiveInt("Godina rođenja");
var phone    = ReadNonEmpty("Broj telefona");

await connection.InvokeAsync("InitSinglePerson", username, city, year, phone);

int _lastSuggestionLength = 0;

while (true)
{
    var input = ReadWithCompletion();

    if (input == null) continue;

    if (input.StartsWith("/block ", StringComparison.OrdinalIgnoreCase))
    {
        var target = input[7..].Trim();
        if (string.IsNullOrEmpty(target))
        {
            AnsiConsole.MarkupLine("[red]Upotreba: /block <username>[/]");
            continue;
        }
        await connection.InvokeAsync("BlockUser", target);
    }
    else if (input == string.Empty && hasPendingLetter)
    {
        AnsiConsole.MarkupLine("[green]Poruka prihvaćena.[/]");
        await connection.InvokeAsync("ConfirmLetter");
    }
}

string ReadNonEmpty(string label)
{
    while (true)
    {
        AnsiConsole.Markup($"[hotpink]{label}:[/] ");
        var value = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(value)) return value;
        AnsiConsole.MarkupLine("[red]Ne sme biti prazno.[/]");
    }
}

int ReadPositiveInt(string label)
{
    while (true)
    {
        AnsiConsole.Markup($"[hotpink]{label}:[/] ");
        var raw = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(raw))
        {
            AnsiConsole.MarkupLine("[red]Ne sme biti prazno.[/]");
            continue;
        }
        if (!int.TryParse(raw, out int value))
        {
            AnsiConsole.MarkupLine($"[red]\"{Markup.Escape(raw)}\" nije broj.[/]");
            continue;
        }
        if (value <= 0)
        {
            AnsiConsole.MarkupLine("[red]Mora biti pozitivan broj.[/]");
            continue;
        }
        return value;
    }
}

string? ReadWithCompletion()
{
    var commands = new[] { "/block " };
    var buffer = new System.Text.StringBuilder();

    AnsiConsole.Markup("[grey]>[/] ");

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
        {
            ClearSuggestion();
            Console.WriteLine();
            return buffer.ToString();
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (buffer.Length > 0)
            {
                ClearSuggestion();
                buffer.Remove(buffer.Length - 1, 1);
                Console.Write("\b \b");
            }
        }
        else if (key.Key == ConsoleKey.Tab)
        {
            var current = buffer.ToString();
            var match = commands.FirstOrDefault(c => c.StartsWith(current, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                var completion = match[current.Length..];
                buffer.Append(completion);
                Console.Write(completion);
            }
        }
        else if (!char.IsControl(key.KeyChar))
        {
            buffer.Append(key.KeyChar);
            Console.Write(key.KeyChar);
        }

        ShowSuggestion(buffer.ToString(), commands);
    }
}

void ShowSuggestion(string input, string[] commands)
{
    ClearSuggestion();

    if (!input.StartsWith('/') || input.Contains(' ')) return;

    var match = commands.FirstOrDefault(c =>
        c.TrimEnd().StartsWith(input, StringComparison.OrdinalIgnoreCase));

    if (match == null) return;

    var hint = match.TrimEnd()[input.Length..] + " <username>";
    var (col, row) = (Console.CursorLeft, Console.CursorTop);

    Console.Write(hint);
    _lastSuggestionLength = hint.Length;
    Console.SetCursorPosition(col, row);
}

void ClearSuggestion()
{
    if (_lastSuggestionLength == 0) return;
    var (col, row) = (Console.CursorLeft, Console.CursorTop);
    Console.Write(new string(' ', _lastSuggestionLength));
    Console.SetCursorPosition(col, row);
    _lastSuggestionLength = 0;
}

record LoveLetter(string FromUsername, string FromCity, int FromYear, string FromPhone, string Message);
