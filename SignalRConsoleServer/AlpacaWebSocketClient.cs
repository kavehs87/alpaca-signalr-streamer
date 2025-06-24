using System;
using System.Text;
using System.Threading.Tasks;
using Websocket.Client;
using Microsoft.AspNetCore.SignalR;

namespace SignalRConsoleServer
{
    public class AlpacaWebSocketClient
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly Uri _webSocketUri;
        private WebsocketClient _client = null!;
        private readonly IHubContext<TickHub> _hubContext;

        public AlpacaWebSocketClient(string webSocketUrl, string apiKey, string apiSecret, IHubContext<TickHub> hubContext)
        {
            _webSocketUri = new Uri(webSocketUrl);
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _hubContext = hubContext;
        }

        public async Task ConnectAsync()
        {
            _client = new WebsocketClient(_webSocketUri);
            _client.DisconnectionHappened.Subscribe(info =>
            {
                Console.WriteLine($"Disconnected: {info.Type}");
            });

            _client.MessageReceived.Subscribe(msg =>
            {
                if (msg.Text != null)
                {
                    HandleMessage(msg.Text);
                }
            });

            await _client.Start();

            Authenticate();

            Console.WriteLine("Connected to Alpaca real-time service.");
        }

        private void Authenticate()
        {
            var authMessage = $"{{\"action\":\"auth\", \"key\":\"{_apiKey}\", \"secret\":\"{_apiSecret}\"}}";
            _client.Send(authMessage);
        }

        public void SubscribeToTicker(string ticker)
        {
            var subscribeMessage = $"{{\"action\":\"subscribe\", \"bars\":[\"{ticker}\"], \"quotes\":[\"{ticker}\"]}}";
            _client.Send(subscribeMessage);
            Console.WriteLine($"Subscribed to ticker: {ticker}");
        }

        private async void HandleMessage(string message)
        {
            Console.WriteLine($"Received message: {message}");

            await _hubContext.Clients.All.SendAsync("ReceiveTickerData", message);
        }

        public void Disconnect()
        {
            _client?.Dispose();
            Console.WriteLine("Disconnected from Alpaca real-time service.");
        }
    }
}