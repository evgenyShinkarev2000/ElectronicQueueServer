using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace WebSocektUser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (Process.GetProcesses(Process.GetCurrentProcess().ProcessName).Length <= 2)
            {
                Process.Start("WebSocketUser.exe");
            }

            StartWebSocket().GetAwaiter().GetResult();
        }

        public static async Task StartWebSocket()
        {
            var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("wss://localhost:44315/ws"), CancellationToken.None);
            Console.WriteLine("websocket conected");
            var send = Task.Run(async () =>
            {
                string? message;
                while ((message = Console.ReadLine()) != null && message != string.Empty)
                {
                    var bytes = Encoding.UTF8.GetBytes(message);
                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "отключился", CancellationToken.None);
            });

            var receive = Receive(client);

            await Task.WhenAll(send, receive);
        }

        public static async Task Receive(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];
            while (true)
            {
                var message = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (message.CloseStatus.HasValue)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }

                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, message.Count));
            }

        }
    }
}