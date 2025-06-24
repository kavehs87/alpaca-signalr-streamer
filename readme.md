# 🐪 Alpaca Real-Time Market Data Pipeline

This project is a two-part C#/.NET 8.0 solution for streaming real-time stock market data using the **Alpaca Market Data API** and distributing it to clients via **SignalR WebSockets**.

---

## 🧩 Project Structure

### `SignalRConsoleServer`
A lightweight ASP.NET Core WebSocket server that:
- Connects to the Alpaca market data feed.
- Subscribes to specific tickers.
- Parses `Quote` messages.
- Broadcasts them to all connected SignalR clients via a `TickHub`.

### `SignalRConsoleClient`
A C# console application that:
- Connects to the server's `TickHub`.
- Listens for incoming market data (quotes or bars).
- Parses and logs/display the messages.
- Can be extended to forward data to cloud services (Azure, etc.).

---

## 🔄 Data Flow

```
[Alpaca Market Data API]
            ↓
AlpacaWebSocketClient (Server)
            ↓
        TickHub (SignalR)
            ↓
AlpacaWebSocketClient (Client)
            ↓
  Logs / Cloud Forwarding
```

---

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Alpaca API Key & Secret
- Internet access (WebSocket endpoints)

### Build & Run (Server)
```bash
cd SignalRConsoleServer
dotnet run
```

### Build & Run (Client)
```bash
cd SignalRConsoleClient
dotnet run
```

---

## ⚙️ Configuration

You may hardcode or externalize:
- Alpaca API key & secret
- Symbols to subscribe to
- Server SignalR endpoint (`/tickHub`)

---

## 🧠 Use Cases

- Feed live market data into Azure Event Hubs or Data Lake
- Monitor real-time quotes for custom alerts
- Build algorithmic trading engines
- Power frontend dashboards with SignalR

---

## 📄 License

This project is licensed under the MIT License.

---

## ✍️ Authors

Developed by Kaveh Sarkhanlou. Contributions welcome!