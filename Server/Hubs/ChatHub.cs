using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class ChatHub : Hub
{
    //отправить сообщение всем
    public async Task ServerSend(string message)
    {
        await Clients.All.SendAsync("ClientRecieveMessage", message);
    }

    //отправить сообщение серверу
    public async Task ServerMessageToConsole(string message)
    {
        Console.WriteLine($"SignalR recieved from client: {message}");
        await Clients.Caller.SendAsync("ClientRecieveMessage",
            $"(from Clients.Caller.SendAsync) Hello client, I recieved: {message}");
    }
}