<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=12&height=220&section=header&text=Chaotic%20Cupid&fontSize=72&fontColor=fff&animation=fadeIn&fontAlignY=42&desc=Real-time%20matchmaking%20%7C%20ASP.NET%20Core%20%26%20SignalR&descSize=18&descAlignY=64&descAlign=50"/>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet"/>
  <img src="https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp"/>
  <img src="https://img.shields.io/badge/SignalR-Real--time-e0457b?style=flat-square"/>
  <img src="https://img.shields.io/badge/Spectre.Console-UI-blueviolet?style=flat-square"/>
  <img src="https://img.shields.io/badge/Platform-Cross--platform-blue?style=flat-square"/>
</p>

<p align="center">
  <a href="#english">English</a> &nbsp;·&nbsp; <a href="#srpski">Srpski</a>
</p>

---

<a name="english"></a>
## [ EN ] &nbsp; Technical Documentation

ChaoticCupid is a real-time matchmaking simulation system built on .NET 8. It demonstrates modern distributed application patterns — asynchronous communication, background processing, and thread-safe state management — through a PubSub architecture powered by ASP.NET Core SignalR.

<br>

### » System Architecture

The project follows a decoupled client-server model using WebSockets for low-latency, bi-directional communication.

#### ◆ ChaoticCupid.Server

The backbone of the system, responsible for orchestration and business logic:

- **Registry Management** — a centralized, thread-safe `PersonRegistry` tracks all active users
- **SignalR Hub** — `CupidHub` manages connection lifecycle and exposes the client-facing interface
- **Background Orchestrator** — `CupidService` runs independently of the request cycle, executing matchmaking every 60 seconds

#### ◆ ChaoticCupid.Client

A terminal-based interface built for clarity:

- **Rich UI** — powered by `Spectre.Console` with colored panels, styled prompts, and a FigletText banner
- **Command Input** — custom keystroke handler with tab-completion for `/block`
- **State Management** — tracks pending letter confirmation and blocked users locally

<br>

### » Technical Implementation

#### ◆ Thread-Safe State

`PersonRegistry` uses manual `lock` blocks to prevent race conditions during concurrent registrations and disconnections, ensuring data integrity throughout the matchmaking cycle.

#### ◆ Matchmaking Algorithm

Candidates are scored against each recipient using a weighted system:

| Factor | Points |
|---|---|
| Same city | +30 |
| Age within ±2 years | +20 |
| Cryptographic random factor | +0 to 100 |

The candidate with the highest score becomes the letter sender. The random element is generated via `RandomNumberGenerator` (`System.Security.Cryptography`) — the modern equivalent of `RNGCryptoServiceProvider`.

#### ◆ Connection Resilience

- On startup, the client retries every 3 seconds until the server responds
- If the server drops during a session, a `Closed` event notifies the user to restart
- Automatic reconnect is intentionally disabled — the server holds state in memory and would not recognize a reconnected client

---

<a name="srpski"></a>
## [ SR ] &nbsp; Dokumentacija

ChaoticCupid je sistem za simulaciju provodadžisanja u realnom vremenu, izgrađen na .NET 8 platformi. Projekat demonstrira moderne obrasce distribuiranih aplikacija — asinhronu komunikaciju, pozadinsku obradu i bezbedno upravljanje stanjem u višenitnom okruženju — kroz PubSub arhitekturu zasnovanu na ASP.NET Core SignalR-u.

<br>

### » Arhitektura Sistema

Projekat koristi razdvojenu klijent-server arhitekturu uz WebSocket konekciju za dvosmernu komunikaciju sa niskim kašnjenjem.

#### ◆ ChaoticCupid.Server

Okosnica sistema zadužena za orkestraciju i poslovnu logiku:

- **Upravljanje Registrom** — centralizovani, thread-safe `PersonRegistry` prati sve aktivne korisnike
- **SignalR Hub** — `CupidHub` upravlja životnim ciklusom konekcija i pruža interfejs za klijente
- **Pozadinski Orkestrator** — `CupidService` radi nezavisno od request ciklusa i izvršava mečovanje svakih 60 sekundi

#### ◆ ChaoticCupid.Client

Terminalni interfejs izgrađen za jasnoću i upotrebljivost:

- **Bogati UI** — `Spectre.Console` sa obojenim panelima, stilizovanim promptovima i FigletText bannerom
- **Unos Komandi** — prilagođeni hendler sa tab-completion podrškom za `/block`
- **Praćenje Stanja** — lokalno upravljanje potvrdom pisama i listom blokiranih korisnika

<br>

### » Tehnička Implementacija

#### ◆ Bezbednost Niti

`PersonRegistry` koristi `lock` blokove za sprečavanje race condition situacija tokom istovremenih registracija i diskonekcija, čuvajući integritet podataka tokom celog ciklusa mečovanja.

#### ◆ Algoritam Mečovanja

Kandidati se boduju u odnosu na svakog primaoca prema sledećem sistemu:

| Faktor | Poeni |
|---|---|
| Isti grad | +30 |
| Godište u rasponu ±2 | +20 |
| Kriptografski nasumični faktor | +0 do 100 |

Kandidat sa najvišim skorom postaje pošiljalac pisma. Nasumični element generiše `RandomNumberGenerator` (`System.Security.Cryptography`) — moderna zamena za `RNGCryptoServiceProvider`.

#### ◆ Otpornost Konekcije

- Pri pokretanju, klijent pokušava konekciju svakih 3 sekunde dok server ne odgovori
- Ako server padne tokom sesije, `Closed` event obaveštava korisnika da ponovo pokrene aplikaciju
- Automatski reconnect je namerno isključen — server čuva stanje u memoriji i ne bi prepoznao ponovo konektovanog klijenta

---

## [ ! ] &nbsp; Getting Started / Pokretanje

### Preduslovi / Prerequisites
- .NET 8.0 SDK

### Pokretanje / Setup

**1. Kloniraj repozitorijum / Clone the repository**
```bash
git clone git@github.com:ljubogdan/chaotic-cupid.git
cd chaotic-cupid
```

**2. Pokreni server / Launch the server**
```bash
dotnet run --project ChaoticCupid.Server
```
Server sluša na `http://localhost:5104`.

**3. Pokreni klijent(e) / Launch client(s)**
```bash
dotnet run --project ChaoticCupid.Client
```
Svaki klijent se pokreće u posebnom terminalu.

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=gradient&customColorList=12&height=120&section=footer"/>
</p>
