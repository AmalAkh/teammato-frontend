

using System.Text.Json;
using Teammato.Abstractions;

namespace Teammato.Services;

using System.Text;
using System.Net.WebSockets;
public class WebSocketService
{
    private static List<Action<WebSocketNotification>> _handlers = new List<Action<WebSocketNotification>>();
    
    public static void AddHandler(Action<WebSocketNotification> handler) => _handlers.Add(handler);
    private static ClientWebSocket _client = new ClientWebSocket();
    
    public static WebSocketState State => _client.State;
    public static async  Task ConnectAsync(Uri uri)
    {
        var accessToken = RestAPIService.GetAccessToken();
        try
        {
            await _client.ConnectAsync(uri, CancellationToken.None);
        }
        catch (WebSocketException e)
        {
            return;
        }

        await _client.SendAsync(Encoding.UTF8.GetBytes(accessToken), WebSocketMessageType.Text, true, CancellationToken.None);
        Task.Run(async ()=>
        {
            while(_client.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                WebSocketNotification notification = JsonSerializer.Deserialize<WebSocketNotification>(message);
                Console.WriteLine("Received: " + message);
                foreach (var handler in _handlers)
                {
                    handler(notification);
                }
            }

            if (_client.State == WebSocketState.Closed)
            {
                Console.WriteLine("Closed");
            }
            
        });
        
        
    }
}