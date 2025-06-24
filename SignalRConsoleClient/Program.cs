using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRClient
{
    class Program
    {
        // Declare the AlpacaWebSocketClient instance at the class level
        private static AlpacaWebSocketClient client = new AlpacaWebSocketClient();

        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/tickhub")  // Replace with your SignalR hub URL
                .Build();

            // Set up handler for incoming ticker data
            connection.On<string>("ReceiveTickerData", async (message) =>
            {
                Console.WriteLine(message);
                await HandleIncomingMessage(message);  // Call method to process the message
            });

            // Reconnection logic
            connection.Closed += async (error) =>
            {
                Console.WriteLine("Connection lost. Trying to reconnect...");
                await Task.Delay(new Random().Next(0, 5) * 1000);  // Wait before reconnecting
                await connection.StartAsync();
                Console.WriteLine("Reconnected to the SignalR hub.");
            };

            try
            {
                // Start the connection
                await connection.StartAsync();
                Console.WriteLine("Connected to the SignalR hub.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SignalR hub: {ex.Message}");
                return;  // Exit if unable to connect
            }

            // Keep the app running
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            // Gracefully stop the SignalR connection
            await connection.StopAsync();
            Console.WriteLine("Disconnected from SignalR hub.");
        }

        static async Task HandleIncomingMessage(string jsonData)
        {
            // Use the existing instance of AlpacaWebSocketClient
            await client.HandleIncomingMessage(jsonData);
        }
    }
}