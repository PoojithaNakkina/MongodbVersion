//using DotNetEnv;
//using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;
//using esyasoft.mobility.CHRGUP.service.api.Interfaces;
//using esyasoft.mobility.CHRGUP.service.api.Services;
//using esyasoft.mobility.CHRGUP.service.core.Helpers;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.persistence.Settings;
////using Microsoft.EntityFrameworkCore;
//using System.Text.Json.Serialization;

//Env.Load();

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.Converters.Add(
//            new JsonStringEnumConverter());
//    });
////builder.Services.AddDbContext<AppDbContext>(options =>
////{
////    var conn = builder.Configuration.GetConnectionString("DBConnection");


////    if (string.IsNullOrEmpty(conn))
////        throw new InvalidOperationException("Connection string not found.");

////    options.UseNpgsql(
////        conn,
////        b => b.MigrationsAssembly("esyasoft.mobility.CHRGUP.service.persistence")
////    );
////});


//var mongoConnectionString =
//    Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");

//var mongoDatabaseName =
//    Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

//if (string.IsNullOrWhiteSpace(mongoConnectionString))
//    throw new InvalidOperationException(
//        "MONGODB_CONNECTION_STRING not found in .env");

//if (string.IsNullOrWhiteSpace(mongoDatabaseName))
//    throw new InvalidOperationException(
//        "MONGODB_DATABASE_NAME not found in .env");

//// 🔹 Register MongoDbSettings
//var mongoSettings = new MongoDbSettings
//{
//    ConnectionString = mongoConnectionString,
//    DatabaseName = mongoDatabaseName
//};

//builder.Services.AddSingleton(mongoSettings);

//// 🔹 Register MongoDbContext
//builder.Services.AddSingleton<MongoDbContext>();




//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddScoped<IChargerService, ChargerService>();
//builder.Services.AddScoped<ILocationService, LocationService>();
//builder.Services.AddScoped<IDriverService, DriverService>();
//builder.Services.AddScoped<IVehicleService, VehicleService>();
//builder.Services.AddScoped<IChargingSessionService, ChargingSessionService>();
//builder.Services.AddScoped<ILogService, LogService>();
//builder.Services.AddScoped<IReservationService, ReservationService>();
//builder.Services.AddScoped<AuditLogger>();

//builder.Services.AddSingleton<RmqPublisher>();

//var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();

//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();


using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.api.Services;
using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.persistence.Settings;
using System.Text.Json.Serialization;

//Env.Load();

Env.Load(Path.Combine(
    Directory.GetCurrentDirectory(),
    "..", "..","..",  ".env"
));

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// MongoDB from .env
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

// Register Mongo settings
builder.Services.AddSingleton(new MongoDbSettings
{
    ConnectionString = mongoConnectionString,
    DatabaseName = mongoDatabaseName
});

// Register Mongo context (Singleton)
builder.Services.AddSingleton<MongoDbContext>();

// Application services
builder.Services.AddScoped<IChargerService, ChargerService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IChargingSessionService, ChargingSessionService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<AuditLogger>();

// RabbitMQ
builder.Services.AddSingleton<RmqPublisher>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
