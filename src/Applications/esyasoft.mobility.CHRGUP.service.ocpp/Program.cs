//using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
//using esyasoft.mobility.CHRGUP.service.ocpp.Services;
//using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
////using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
////using OcppMicroservice.Watchdog;
//////using OcppMicroservice.WebSockets;
////using esyasoft.mobility.CHRGUP.service.ocpp.Data;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.persistence.Settings;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddHostedService<ChargerWatchdog>();

////builder.Services.AddDbContext<OcppDbContext>(opt =>
////    opt.UseNpgsql(builder.Configuration["OcppDb:ConnectionString"]));
////builder.Services.AddDbContext<AppDbContext>(options =>
////{
////    options.UseNpgsql(
////        builder.Configuration.GetConnectionString("DBConnection")
////    );
////});

//builder.Services.Configure<MongoDbSettings>(
//    builder.Configuration.GetSection("MongoDb"));

//builder.Services.AddScoped<ChargerAuthService>();

//var app = builder.Build();

//app.UseWebSockets();

//app.Map("/ws", wsApp =>
//{
//    wsApp.UseMiddleware<WebSocketMiddleware>();
//});

//_ = RabbitMqConnection.Channel;

//var commandConsumer = new RabbitMqConsumer(RabbitMqConnection.Channel);
//commandConsumer.Start();

//app.Run();


using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.Services;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.persistence.Settings;

//Env.Load();

Env.Load(Path.Combine(
    Directory.GetCurrentDirectory(),
    "..", "..","..", ".env"
));

var builder = WebApplication.CreateBuilder(args);

// 🔹 Background services
builder.Services.AddHostedService<ChargerWatchdog>();

// 🔹 MongoDB from .env
var mongoConnectionString =
    Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");

var mongoDatabaseName =
    Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

if (string.IsNullOrWhiteSpace(mongoConnectionString))
    throw new InvalidOperationException(
        "MONGODB_CONNECTION_STRING not found in .env");

if (string.IsNullOrWhiteSpace(mongoDatabaseName))
    throw new InvalidOperationException(
        "MONGODB_DATABASE_NAME not found in .env");

// 🔹 Register Mongo settings
builder.Services.AddSingleton(new MongoDbSettings
{
    ConnectionString = mongoConnectionString,
    DatabaseName = mongoDatabaseName
});

// 🔹 Register Mongo context
builder.Services.AddSingleton<MongoDbContext>();

// 🔹 OCPP services
builder.Services.AddScoped<ChargerAuthService>();

var app = builder.Build();

// 🔹 WebSocket pipeline
app.UseWebSockets();

app.Map("/ws", wsApp =>
{
    wsApp.UseMiddleware<WebSocketMiddleware>();
});

// 🔹 RabbitMQ startup
_ = RabbitMqConnection.Channel;

var commandConsumer = new RabbitMqConsumer(RabbitMqConnection.Channel);
commandConsumer.Start();

app.Run();

