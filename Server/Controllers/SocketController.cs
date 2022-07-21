using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SocketController : ControllerBase
{
    [HttpGet("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            using var cancellationTokenSource = new CancellationTokenSource();

            await SocketRecieveOneAsync(webSocket, cancellationTokenSource.Token);

            await webSocket.SendAsync(Encoding.UTF8.GetBytes("Hello from socket, client!"), WebSocketMessageType.Text,
                WebSocketMessageFlags.EndOfMessage, cancellationTokenSource.Token);

            await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "byServer",
                cancellationTokenSource.Token);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task SocketRecieveOneAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var data = new byte[256]; // буфер для чтения
        var builder = new StringBuilder();

        WebSocketReceiveResult result;
        if (webSocket.State == WebSocketState.Open)
        {
            do
            {
                result = await webSocket.ReceiveAsync(data, cancellationToken);
                builder.Append(Encoding.UTF8.GetString(data, 0, result.Count));
            } while (!result.EndOfMessage);

            Console.WriteLine($"RecievedFromSocket: {builder}");
            builder.Clear();
        }
    }
}