using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    public static string MessageStr { get; set; } = "Ok";

    [HttpGet]
    [Route("GetMyModel")]
    public MyModel GetMyModel([FromQuery] int? id)
    {
        Console.WriteLine($"{nameof(GetMyModel)}, id:{id}");
        return new MyModel
        {
            Id = id ?? Random.Shared.Next(0, 100),
            Message = MessageStr
        };
    }

    [HttpPut("PutMyModel")]
    public MyModel SetNewMessage(MyModel myModel)
    {
        Console.WriteLine($"{nameof(GetMyModel)}, myModel:{JsonSerializer.Serialize(myModel)}");
        MessageStr = myModel.Message;
        return myModel;
    }
}