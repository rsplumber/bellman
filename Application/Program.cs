using Application;
using Core;
using Data;
using Data.InMemory;
using FastEndpoints;
using FastEndpoints.Swagger;
using KunderaNet.FastEndpoints.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();
builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.ListenAnyIP(5111, _ => { });
    // options.ListenAnyIP(5112, listenOptions =>
    // {
    //     listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
    //     listenOptions.UseHttps();
    // });
});

builder.Services.AddCors();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(settings =>
{
    settings.Title = "Bellman notification center - WebApi";
    settings.DocumentName = "v1";
    settings.Version = "v1";
    settings.AddKunderaAuth();
}, addJWTBearerAuth: false, maxEndpointVersion: 1);

builder.Services.AddAuthentication(KunderaDefaults.Scheme)
    .AddKundera(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddCap(options =>
{
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

builder.Services.AddCore(builder.Configuration);
builder.Services.AddData(builder.Configuration);
builder.Services.AddInMemoryData();
builder.Services.AddNotificationService(builder.Configuration);

var app = builder.Build();

app.UseNotificationCenter(builder.Configuration);

app.UseCors(b => b.AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Versioning.Prefix = "v";
    config.Versioning.PrependToRoute = true;
    config.Endpoints.Filter = ep => ep.EndpointTags?.Contains("hidden") is not true;
});

app.UseAuthentication();
app.UseAuthorization();

// if (app.Environment.IsDevelopment())
// {
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());
// }


await app.RunAsync();