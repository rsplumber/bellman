using System.Text.Json;
using Application;
using Core;
using Data;
using Data.InMemory;
using Elastic.Apm.NetCoreAll;
using FastEndpoints;
using FastEndpoints.Swagger;
using KunderaNet.FastEndpoints.Authorization;
using KunderaNet.Services.Authorization.Http;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();
builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.ListenAnyIP(5234, _ => { });
    // options.ListenAnyIP(5112, listenOptions =>
    // {
    //     listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
    //     listenOptions.UseHttps();
    // });
});
builder.Services.AddHealthChecks();
builder.Services.AddCors();
builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(settings =>
{
    settings.DocumentSettings = generatorSettings =>
    {
        generatorSettings.Title = "Bellman notification center - WebApi";
        generatorSettings.DocumentName = "v1";
        generatorSettings.Version = "v1";
        generatorSettings.AddKunderaAuth();
    };
    settings.EnableJWTBearerAuth = false;
    settings.MaxEndpointVersion = 1;
});

builder.Services.AddAuthentication(KunderaDefaults.Scheme)
    .AddKundera(builder.Configuration, k => k.UseHttpService(builder.Configuration));
builder.Services.AddAuthorization();
builder.Services.AddCore(builder.Configuration);
builder.Services.AddData(builder.Configuration);
builder.Services.AddInMemoryData();
builder.Services.AddNotificationService(builder.Configuration);
builder.Services.AddCap(options =>
{
    options.FailedRetryCount = 3;
    options.FailedRetryInterval = 30;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
    options.SucceedMessageExpiredAfter = 30;
    options.FailedMessageExpiredAfter = 60 * 20;

    options.UseRabbitMQ(op =>
    {
        op.HostName = builder.Configuration.GetValue<string>("RabbitMQ:HostName") ?? throw new ArgumentNullException("RabbitMQ:HostName", "Enter RabbitMQ:HostName in app settings");
        op.UserName = builder.Configuration.GetValue<string>("RabbitMQ:UserName") ?? throw new ArgumentNullException("RabbitMQ:UserName", "Enter RabbitMQ:UserName in app settings");
        op.Password = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? throw new ArgumentNullException("RabbitMQ:Password", "Enter RabbitMQ:UserName in app settings");
        op.ExchangeName = builder.Configuration.GetValue<string>("RabbitMQ:ExchangeName") ?? throw new ArgumentNullException("RabbitMQ:ExchangeName", "Enter RabbitMQ:ExchangeName in app settings");
    });
    options.UsePostgreSql(sqlOptions =>
    {
        sqlOptions.ConnectionString = builder.Configuration.GetConnectionString("default") ?? throw new ArgumentNullException("connectionString", "Enter connection string in app settings");
        sqlOptions.Schema = "events";
    });
});
var app = builder.Build();

app.UseNotificationCenter(builder.Configuration);

app.UseCors(b => b.AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.UseHealthChecks("/health");
app.UseAuthentication();
app.UseAuthorization();
app.UseAllElasticApm(builder.Configuration);
app.UseFastEndpoints(config =>
{
    config.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    config.Endpoints.RoutePrefix = "api";
    config.Versioning.Prefix = "v";
    config.Versioning.PrependToRoute = true;
});


// if (app.Environment.IsDevelopment())
// {
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());
// }


await app.RunAsync();