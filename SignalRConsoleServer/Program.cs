using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignalRConsoleServer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SignalRConsoleServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            var hubContext = host.Services.GetRequiredService<Microsoft.AspNetCore.SignalR.IHubContext<TickHub>>();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddIniFile("config.ini")
                .Build();

            string? apiKey = config["Alpaca:ApiKey"];
            string? apiSecret = config["Alpaca:ApiSecret"];
            string? webSocketUrl = config["Alpaca:WebSocketUrl"];
            string? instrument = config["Alpaca:Instrument"];

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret) || string.IsNullOrWhiteSpace(webSocketUrl) || string.IsNullOrWhiteSpace(instrument))
            {
                throw new InvalidOperationException("API Key, API Secret, or WebSocket URL is missing in the configuration file.");
            }

            var alpacaClient = new AlpacaWebSocketClient(webSocketUrl, apiKey, apiSecret, hubContext);

            await alpacaClient.ConnectAsync();
            alpacaClient.SubscribeToTicker(instrument);

            await host.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddIniFile("config.ini")
                .Build();

            string hostIp = config["Server:HostIp"] ?? "0.0.0.0";
            string port = config["Server:Port"] ?? "8080";

            string url = $"http://{hostIp}:{port}";

            return WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls(url);
        }
    }
}