# ChaoticCupid

ChaoticCupid is a robust, real-time matchmaking simulation system built using the .NET 8 framework. It serves as a practical demonstration of modern distributed application patterns, focusing on asynchronous communication, background processing, and thread-safe state management.

## System Architecture

The project follows a decoupled client-server architecture, leveraging the power of WebSockets through ASP.NET Core SignalR for low-latency, bi-directional communication.

### Component Breakdown

#### 1. ChaoticCupid.Server
The backbone of the system, responsible for orchestration and business logic:
- **Registry Management**: Utilizes a centralized, thread-safe `PersonRegistry` to track active users.
- **SignalR Hub**: The `CupidHub` manages lifecycle events (connection/disconnection) and provides a secure interface for client interactions.
- **Asynchronous Orchestrator**: A `BackgroundService` implementation (`CupidService`) that operates independently of the request-response cycle to perform heavy computational tasks (matchmaking).

#### 2. ChaoticCupid.Client
A high-performance terminal-based interface:
- **Interactive UI**: Built with `Spectre.Console` to provide a rich, user-friendly experience in a CLI environment.
- **Command Dispatcher**: Implements a custom input handler with support for tab-completion and command-line arguments (e.g., `/block`).
- **State Awareness**: Maintains a local state to handle complex flows like message confirmation and blocking logic.

## Technical Implementation Details

### Thread-Safe State Management
To ensure reliability in a multi-threaded environment, the `PersonRegistry` implements manual locking mechanisms. This prevents race conditions during high-concurrency registration and disconnection phases, ensuring data integrity for the matchmaking algorithm.

### Matchmaking Heuristics
The matching algorithm is designed to simulate complex social dynamics. It processes candidates based on a weighted scoring system:
- **Regional Affinity (30 pts)**: Prioritizes users within the same geographic location.
- **Age Compatibility (20 pts)**: Favors profiles within a specific age range (+/- 2 years).
- **Stochastic Factor (0-100 pts)**: Introduces a randomized element to prevent deterministic outcomes and enhance the "chaotic" nature of the service.

### Communication Protocol
The system defines clear contracts through interfaces:
- `IPersonInterface`: Defines actions a client can perform (registration, blocking, confirmation).
- `ICupidInterface`: Defines the internal server-side orchestration capabilities.

## Technical Stack

- **Framework**: .NET 8.0
- **Real-time Engine**: ASP.NET Core SignalR
- **CLI Rendering**: Spectre.Console
- **Concurrency**: TPL (Task Parallel Library) and Background Tasks
- **Logging**: Integrated ILogger for structured diagnostic output

## Project Structure

```text
ChaoticCupid/
├── ChaoticCupid.Server/
│   ├── Hubs/            # SignalR communication logic
│   ├── Services/        # Background tasks and data registries
│   ├── Models/          # Core domain entities
│   └── Interfaces/      # Abstractions and contracts
└── ChaoticCupid.Client/
    └── Program.cs       # Client-side logic and UI rendering
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK

### Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd chaotic-cupid
   ```

2. **Launch the Server**
   ```bash
   dotnet run --project ChaoticCupid.Server
   ```
   The server will start listening on `http://localhost:5104`.

3. **Launch the Client**
   ```bash
   dotnet run --project ChaoticCupid.Client
   ```
   Open multiple terminal windows and repeat this step to simulate multiple users.

## Usage and Commands

Once the client is connected, you can interact with the system using the following flow:
- **Initialization**: Enter your name, city, and year of birth as prompted.
- **Receiving Letters**: The server will periodically send match suggestions.
- **Confirmation**: Press `ENTER` when prompted to confirm receipt of a letter.
- **Blocking**: Type `/block <username>` to prevent future matches with a specific individual.
- **Autocomplete**: Use the `TAB` key to autocomplete commands like `/block`.
