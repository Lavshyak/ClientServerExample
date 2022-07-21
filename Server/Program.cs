using Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//signalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//sockets
app.UseWebSockets();

//signalR
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapHub<ChatHub>("/ChatHub"); });


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();