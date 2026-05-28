# ChaoticCupid

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![SignalR](https://img.shields.io/badge/SignalR-Real--time-orange?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Cross--platform-blue?style=flat-square)

[English](#english) | [Srpski](#srpski)

---

<a name="english"></a>
## [ EN ] Technical Documentation

ChaoticCupid is a robust, real-time matchmaking simulation system built using the .NET 8 framework. It serves as a practical demonstration of modern distributed application patterns, focusing on asynchronous communication, background processing, and thread-safe state management.

### » System Architecture

The project follows a decoupled client-server architecture, leveraging the power of WebSockets through ASP.NET Core SignalR for low-latency, bi-directional communication.

#### 1. ChaoticCupid.Server
The backbone of the system, responsible for orchestration and business logic:
*   **Registry Management**: Utilizes a centralized, thread-safe `PersonRegistry` to track active users.
*   **SignalR Hub**: The `CupidHub` manages lifecycle events (connection/disconnection) and provides a secure interface for client interactions.
*   **Asynchronous Orchestrator**: A `BackgroundService` implementation (`CupidService`) that operates independently of the request-response cycle to perform heavy computational tasks (matchmaking).

#### 2. ChaoticCupid.Client
A high-performance terminal-based interface:
*   **Interactive UI**: Built with `Spectre.Console` to provide a rich, user-friendly experience in a CLI environment.
*   **Command Dispatcher**: Implements a custom input handler with support for tab-completion and command-line arguments (e.g., `/block`).
*   **State Awareness**: Maintains a local state to handle complex flows like message confirmation and blocking logic.

### » Technical Implementation Details

#### ◈ Thread-Safe State Management
To ensure reliability in a multi-threaded environment, the `PersonRegistry` implements manual locking mechanisms. This prevents race conditions during high-concurrency registration and disconnection phases, ensuring data integrity for the matchmaking algorithm.

#### ◈ Matchmaking Heuristics
The matching algorithm is designed to simulate complex social dynamics. It processes candidates based on a weighted scoring system:
*   **Regional Affinity (30 pts)**: Prioritizes users within the same geographic location.
*   **Age Compatibility (20 pts)**: Favors profiles within a specific age range (+/- 2 years).
*   **Stochastic Factor (0-100 pts)**: Introduces a randomized element to prevent deterministic outcomes and enhance the "chaotic" nature of the service.

---

<a name="srpski"></a>
## [ SR ] Dokumentacija na srpskom jeziku

ChaoticCupid je robustan sistem za simulaciju provodadžisanja (matchmaking) u realnom vremenu, izgrađen korišćenjem .NET 8 radnog okvira. Projekat predstavlja praktičnu demonstraciju modernih obrazaca distribuiranih aplikacija, sa fokusom na asinhronu komunikaciju, pozadinsku obradu podataka i bezbedno upravljanje stanjima u višenitnom okruženju.

### » Arhitektura Sistema

Projekat se oslanja na razdvojenu klijent-server arhitekturu, koristeći snagu WebSockets tehnologije putem ASP.NET Core SignalR-a za dvosmernu komunikaciju sa niskim kašnjenjem.

#### 1. ChaoticCupid.Server
Okosnica sistema zadužena za orkestraciju i poslovnu logiku:
*   **Upravljanje Registrom**: Koristi centralizovani, thread-safe `PersonRegistry` za praćenje aktivnih korisnika.
*   **SignalR Hub**: `CupidHub` upravlja životnim ciklusom konekcija (povezivanje/prekidanje) i pruža siguran interfejs za interakciju sa klijentima.
*   **Asinhroni Orkestrator**: Implementacija `BackgroundService` klase (`CupidService`) koja radi nezavisno od request-response ciklusa kako bi izvršavala računski intenzivne zadatke (mečovanje).

#### 2. ChaoticCupid.Client
Terminalni interfejs visokih performansi:
*   **Interaktivni UI**: Izgrađen pomoću `Spectre.Console` biblioteke kako bi pružio bogato i intuitivno korisničko iskustvo u CLI okruženju.
*   **Upravljanje Komandama**: Implementira prilagođeni hendler za unos sa podrškom za tab-completion i argumente komandne linije (npr. `/block`).
*   **Praćenje Stanja**: Održava lokalno stanje klijenta za upravljanje kompleksnim tokovima poput potvrde prijema poruka i logike blokiranja.

### » Detalji Tehničke Implementacije

#### ◈ Bezbedno Upravljanje Nitima (Thread-Safety)
Kako bi se osigurala pouzdanost u višenitnom okruženju, `PersonRegistry` implementira manuelne mehanizme zaključavanja (locking). Ovo sprečava pojavu "race condition" situacija tokom faza registracije i diskonekcije, osiguravajući integritet podataka za algoritam mečovanja.

#### ◈ Heuristika Mečovanja
Algoritam za pronalaženje parova dizajniran je da simulira kompleksnu društvenu dinamiku. Procesuiranje kandidata vrši se na osnovu sistema težinskih poena:
*   **Regionalna Sličnost (30 poena)**: Prioritet imaju korisnici iz iste geografske lokacije (grada).
*   **Kompatibilnost Godišta (20 poena)**: Favorizuju se profili unutar određenog raspona godina (+/- 2 godine).
*   **Stohastički Faktor (0-100 poena)**: Uvodi element slučajnosti kako bi se izbegli deterministički ishodi i naglasila "haotična" priroda servisa.

---

## [ ! ] Getting Started / Kako početi

### Prerequisites / Preduslovi
*   .NET 8.0 SDK

### Setup Instructions / Uputstvo za podešavanje

1. **Clone the Repository / Kloniraj repozitorijum**
   ```bash
   git clone git@github.com:ljubogdan/chaotic-cupid.git
   cd chaotic-cupid
   ```

2. **Launch the Server / Pokreni Server**
   ```bash
   dotnet run --project ChaoticCupid.Server
   ```
   The server will start listening on `http://localhost:5104`.

3. **Launch the Client / Pokreni Klijent**
   ```bash
   dotnet run --project ChaoticCupid.Client
   ```
