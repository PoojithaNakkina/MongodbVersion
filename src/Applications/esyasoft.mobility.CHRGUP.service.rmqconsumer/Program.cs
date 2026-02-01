//using esyasoft.mobility.CHRGUP.service.core.Helpers;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.persistence.Settings;
//using esyasoft.mobility.CHRGUP.service.rmqconsumer;
//using esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers;
//using esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging;
//using Microsoft.EntityFrameworkCore;

//var builder = Host.CreateApplicationBuilder(args);

////builder.Services.AddDbContext<AppDbContext>(options =>
////{
////    options.UseNpgsql(
////        builder.Configuration.GetConnectionString("DBConnection")
////    );
////});

//builder.Services.Configure<MongoDbSettings>(
//    builder.Configuration.GetSection("MongoDb"));

//builder.Services.AddScoped<AuditLogger>();
//builder.Services.AddScoped<AuthEvtHandler>();
//builder.Services.AddScoped<StatusEvtHandler>();
//builder.Services.AddScoped<MeterValueEvtHandler>();
//builder.Services.AddScoped<TransactionEvtHandler>();

//builder.Services.AddSingleton<RmqPublisher>();
//builder.Services.AddSingleton<RmqConsumer>();

//builder.Services.AddHostedService<Worker>();

//var host = builder.Build();
//host.Run();


using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.persistence.Settings;
using esyasoft.mobility.CHRGUP.service.rmqconsumer;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging;

//Env.Load();

Env.Load(Path.Combine(
    Directory.GetCurrentDirectory(),
    "..", "..","..", ".env"
));

var builder = Host.CreateApplicationBuilder(args);

// ?? MongoDB from .env
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

// ?? Register Mongo settings
builder.Services.AddSingleton(new MongoDbSettings
{
    ConnectionString = mongoConnectionString,
    DatabaseName = mongoDatabaseName
});

// ?? Register Mongo context
builder.Services.AddSingleton<MongoDbContext>();

// ?? Application services
builder.Services.AddScoped<AuditLogger>();

builder.Services.AddScoped<AuthEvtHandler>();
builder.Services.AddScoped<StatusEvtHandler>();
builder.Services.AddScoped<MeterValueEvtHandler>();
builder.Services.AddScoped<TransactionEvtHandler>();

// ?? RabbitMQ
builder.Services.AddSingleton<RmqPublisher>();
builder.Services.AddSingleton<RmqConsumer>();

// ?? Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
