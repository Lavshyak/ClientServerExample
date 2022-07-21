using System.Net.Mime;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using SharedModels;

namespace Client;

internal class Program
{
    private const string Host = "localhost:7230";

    private static async Task Main()
    {
        await HttpClientMethod();
        await SignalR();
        await Socket();

        //подождать на всякий случай, чтоб запросы все отправились и принялись
        await Task.Delay(2000);
        Console.WriteLine("end");
    }

    //HttpClient
    private static async Task HttpClientMethod()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri($"https://{Host}/Api/");
        await Send(httpClient);
        await Send(httpClient, 10);
        var model = new MyModel
        {
            Id = 13,
            Message = "NewMessage"
        };
        await SetNewMessage(httpClient, model);
    }

    private static async Task Send(HttpClient httpClient, int? id = null)
    {
        var requestUrl = "GetMyModel";
        if (id != null)
            requestUrl += $"?id={id}";

        using var response = await httpClient.GetAsync(requestUrl);

        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    private static async Task SetNewMessage(HttpClient httpClient, MyModel myModel)
    {
        var requestUrl = "PutMyModel";

        var content = new StringContent(
            JsonSerializer.Serialize(myModel), Encoding.UTF8, MediaTypeNames.Application.Json);

        using var response = await httpClient.PutAsync(requestUrl, content);

        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    //SignalR
    private static async Task SignalR()
    {
        //Microsoft.AspNetCore.SignalR.Client nuget
        var connection = new HubConnectionBuilder()
            .WithUrl($"https://{Host}/ChatHub")
            .Build();

        //принять сообщение от сервера
        connection.On<string>("ClientRecieveMessage",
            message => { Console.WriteLine($"SignalR recieved from server: {message}"); });

        await connection.StartAsync();
        await connection.InvokeAsync("ServerMessageToConsole", "HelloServer");
    }

    //Socket
    private static async Task Socket()
    {
        ClientWebSocket clientWebSocket = new();

        using CancellationTokenSource cancellationTokenSource = new();

        await clientWebSocket.ConnectAsync(new Uri($"wss://{Host}/ws"), cancellationTokenSource.Token);
        var message = "Hello from socket, server!";
        var bytes = Encoding.UTF8.GetBytes(message);
        await clientWebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        SocketRecieving(clientWebSocket, cancellationTokenSource.Token);
    }

    private static async void SocketRecieving(ClientWebSocket clientWebSocket, CancellationToken cancellationToken)
    {
        try
        {
            var data = new byte[256]; // буфер для ответа
            var builder = new StringBuilder();

            WebSocketReceiveResult result;
            do
            {
                do
                {
                    result = await clientWebSocket.ReceiveAsync(data, cancellationToken);
                    builder.Append(Encoding.UTF8.GetString(data, 0, result.Count));
                } while (!result.EndOfMessage);

                Console.WriteLine($"RecievedFromSocket: {builder}");
                builder.Clear();
            } while (clientWebSocket.State == WebSocketState.Open);

            Console.WriteLine(
                $"Socked closed: '{result.CloseStatus.ToString()}'. Description: '{result.CloseStatusDescription}'");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}