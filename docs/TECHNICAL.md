# 🏗️ Bellman Technical Documentation

## Architecture Overview

Bellman is built using Clean Architecture principles with a modular, event-driven design that separates concerns across multiple layers.

### Layer Architecture

```
┌─────────────────────────────────────┐
│           Application Layer         │
│  ├── FastEndpoints (API)           │
│  ├── Service Orchestration         │
│  ├── Authentication (KunderaNet)   │
│  └── Swagger Documentation         │
├─────────────────────────────────────┤
│           Core Layer               │
│  ├── Domain Entities               │
│  ├── Business Logic                │
│  ├── Abstractions/Interfaces       │
│  ├── Events & Exceptions           │
│  └── Domain Services               │
├─────────────────────────────────────┤
│           Infrastructure Layer     │
│  ├── Data.Sql (PostgreSQL)         │
│  ├── Data.InMemory (Testing)       │
│  ├── Providers (External APIs)     │
│  └── Queries (CQRS Queries)        │
└─────────────────────────────────────┘
```

### Event-Driven Architecture

Bellman uses DotNetCore.CAP for reliable event-driven communication:

```
API Request → NotificationService → CAP Publisher → RabbitMQ → CAP Consumer → Provider → External API
                                       ↓                           ↓
                                 Event Store (PostgreSQL)    Event Store (PostgreSQL)
                                       ↓                           ↓
                                 Monitoring (Elastic APM)    Monitoring (Elastic APM)
```

## Core Domain Model

### Notification Entity

```csharp
public class Notification
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!; // "sms", "email"
    public string From { get; set; } = default!; // Provider name
    public string Content { get; set; } = default!;
    public List<string> To { get; set; } = new();
    public int Retry { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? FailedAt { get; set; }

    public void IncrementRetry() => Retry++;
}
```

### Provider Entity

```csharp
public class Provider
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!; // "sms", "email"
    public ProviderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### Event Model

```csharp
// Events published via CAP
public record SendNotificationEvent(
    Guid RequestId,
    string Content,
    string[] To,
    string Provider
) : SendNotificationEvent(EventName)
{
    public const string EventName = "bellman.notification.send";
}

public record NotificationSentEvent(Guid Id) : NotificationSentEvent(EventName)
{
    public const string EventName = "bellman.notification.sent";
}

public record NotificationFailedEvent(Guid Id) : NotificationFailedEvent(EventName)
{
    public const string EventName = "bellman.notification.failed";
}
```

## Provider Abstraction Pattern

### AbstractNotificationManagement

All notification providers implement this abstract base class:

```csharp
public abstract class AbstractNotificationManagement
{
    protected readonly ICapPublisher _eventBus;
    protected readonly INotificationRepository _notificationRepository;

    protected AbstractNotificationManagement(
        ICapPublisher eventBus,
        INotificationRepository notificationRepository)
    {
        _eventBus = eventBus;
        _notificationRepository = notificationRepository;
    }

    public abstract string ProviderName { get; }
    public abstract string ProviderType { get; }
    protected abstract int MaximumRetryCount { get; }

    protected abstract Task<bool> SendNotificationAsync(
        string content,
        string to,
        CancellationToken cancellationToken);

    protected abstract Task<bool> SendBatchNotificationAsync(
        string content,
        string[] to,
        CancellationToken cancellationToken);

    protected virtual void Validate(string content, string to) { }
    protected virtual void ValidateBatch(string content, string[] to) { }

    public async Task SendAsync(SendNotificationRequest req, CancellationToken ct)
    {
        var notification = await GetOrAddNotification(req, ct);

        if (MaximumRetryReached(notification))
        {
            await RaiseFailedEventAsync(notification, ct);
            return;
        }

        bool success = IsBatchRequest()
            ? await SendBatchNotificationAsync(req.Content, req.To, ct)
            : await SendNotificationAsync(req.Content, req.To[0], ct);

        if (!success)
        {
            await RaiseSendEventAsync(req, ct);
            return;
        }

        await RaiseSentEventAsync(req.Id, ct);

        bool IsBatchRequest() => req.To.Length > 1;
    }

    // ... implementation details
}
```

## Data Access Layer

### Repository Pattern Implementation

#### INotificationRepository

```csharp
public interface INotificationRepository
{
    Task<Notification?> FindAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task UpdateAsync(Notification notification, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetByStatusAsync(
        NotificationStatus status,
        CancellationToken ct = default);
}
```

#### IProviderRepository

```csharp
public interface IProviderRepository
{
    Task<Provider?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<Provider?> FindByTypeAsync(string type, CancellationToken ct = default);
    Task<IEnumerable<Provider>> GetEnabledProvidersAsync(
        string type,
        CancellationToken ct = default);
    Task AddAsync(Provider provider, CancellationToken ct = default);
    Task UpdateAsync(Provider provider, CancellationToken ct = default);
}
```

### Query Pattern (CQRS Read Models)

#### INotificationListQuery

```csharp
public interface INotificationListQuery
{
    Task<IEnumerable<NotificationResponse>> GetNotificationsAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
}
```

#### NotificationResponse

```csharp
public record NotificationResponse(
    Guid Id,
    string Type,
    string Provider,
    string Content,
    IReadOnlyList<string> Recipients,
    int RetryCount,
    NotificationStatus Status,
    DateTime CreatedAt,
    DateTime? SentAt,
    DateTime? FailedAt
);
```

## API Layer (FastEndpoints)

### Endpoint Structure

```csharp
// Notification Send Endpoint
file sealed class Endpoint : Endpoint<SendNotification>
{
    private readonly INotificationService _service;

    public Endpoint(INotificationService service) => _service = service;

    public override void Configure()
    {
        Post("notifications/send");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(
        SendNotification req,
        CancellationToken ct)
    {
        await _service.SendAsync(req, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

// Request/Response Models
public record SendNotification(
    string Content,
    string[] To,
    string Type,
    string? Provider = null
);

// Validation
file sealed class RequestValidator : Validator<SendNotification>
{
    public RequestValidator()
    {
        RuleFor(x => x.To).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
    }
}

// Swagger Documentation
file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Send batch notification";
        Description = "Send batch notification to specified recipients";
    }
}
```

## Service Layer

### NotificationService

```csharp
internal sealed class NotificationService : INotificationService
{
    private readonly ICapPublisher _eventBus;
    private readonly IProviderSelectionService _providerSelectionService;

    public NotificationService(
        IProviderSelectionService providerSelectionService,
        ICapPublisher eventBus)
    {
        _providerSelectionService = providerSelectionService;
        _eventBus = eventBus;
    }

    public async Task SendAsync(
        SendNotification request,
        CancellationToken cancellationToken = default)
    {
        Provider? provider = request.Provider is not null
            ? await _providerSelectionService.ResolveByNameAsync(request.Provider, cancellationToken)
            : await _providerSelectionService.ResolveByTypeAsync(request.Type, cancellationToken);

        if (provider is null) throw new ProviderNotFoundException();
        if (provider.Status is not ProviderStatus.Enable) throw new ProviderDisabledException();

        await _eventBus.PublishAsync(
            $"{SendNotificationEvent.EventName}.{provider.Type}.{provider.Name}",
            new SendNotificationEvent
            {
                Content = request.Content,
                To = request.To,
                Provider = provider.Name
            },
            cancellationToken: cancellationToken);
    }
}
```

### ProviderSelectionService

```csharp
internal sealed class ProviderSelectionService : IProviderSelectionService
{
    private readonly IProviderRepository _repository;

    public ProviderSelectionService(IProviderRepository repository)
        => _repository = repository;

    public async Task<Provider?> ResolveByNameAsync(
        string name,
        CancellationToken ct = default)
        => await _repository.FindByNameAsync(name, ct);

    public async Task<Provider?> ResolveByTypeAsync(
        string type,
        CancellationToken ct = default)
    {
        var providers = await _repository.GetEnabledProvidersAsync(type, ct);
        return providers.FirstOrDefault(); // Simple selection strategy
    }
}
```

## Database Schema

### PostgreSQL Schema (via EF Core)

```sql
-- Notifications table
CREATE TABLE "Notifications" (
    "Id" uuid NOT NULL,
    "Type" text NOT NULL,
    "From" text NOT NULL,
    "Content" text NOT NULL,
    "To" text[] NOT NULL,
    "Retry" integer NOT NULL DEFAULT 0,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "SentAt" timestamp with time zone NULL,
    "FailedAt" timestamp with time zone NULL,
    CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id")
);

-- Providers table
CREATE TABLE "Providers" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Type" text NOT NULL,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    CONSTRAINT "PK_Providers" PRIMARY KEY ("Id")
);

-- CAP Event Store tables (auto-generated)
CREATE TABLE "cap.published" (...);
CREATE TABLE "cap.received" (...);
```

### EF Core Configuration

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; } = default!;
    public DbSet<Provider> Providers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.To)
                .HasColumnType("text[]");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Type);
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasIndex(e => new { e.Name, e.Type })
                .IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}
```

## Provider Implementations

### SMS Provider Example (Magfa)

```csharp
internal sealed class SendNotificationManagement : AbstractNotificationManagement
{
    private const string Username = "sarmaye_41925";
    private const string Password = "YEfVjZSomtLHIPKW";
    private const string SenderNumber = "98300041925";
    private const string ApiUrl = "http/sms/v2/send";
    private readonly HttpClient _client;

    public SendNotificationManagement(
        ICapPublisher capPublisher,
        INotificationRepository notificationRepository,
        IHttpClientFactory clientFactory)
        : base(capPublisher, notificationRepository)
    {
        _client = clientFactory.CreateClient(ProviderName);
        _client.DefaultRequestHeaders.Add("Username", Username);
        _client.DefaultRequestHeaders.Add("Password", Password);
    }

    public override string ProviderName => "magfa";
    public override string ProviderType => "sms";
    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> SendNotificationAsync(
        string content,
        string to,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync(ApiUrl, new
        {
            senders = new List<string> { SenderNumber },
            messages = new List<string> { content },
            recipients = new[] { to },
        }, cancellationToken);

        return response.IsSuccessStatusCode;
    }

    protected override async Task<bool> SendBatchNotificationAsync(
        string content,
        string[] to,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync(ApiUrl, new
        {
            senders = new List<string> { SenderNumber },
            messages = new List<string> { content },
            recipients = to,
        }, cancellationToken);

        return response.IsSuccessStatusCode;
    }
}
```

## Dependency Injection Configuration

### Core Services

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Domain Services
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IProviderSelectionService, ProviderSelectionService>();
        services.AddScoped<IProviderService, ProviderService>();

        // Repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IProviderRepository, ProviderRepository>();

        return services;
    }
}
```

### Data Layer Registration

```csharp
// PostgreSQL
public static IServiceCollection AddData(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(
            configuration.GetConnectionString("Default"),
            b => b.MigrationsAssembly("Data.Sql")));

    services.AddScoped<INotificationRepository, NotificationRepository>();
    services.AddScoped<IProviderRepository, ProviderRepository>();

    return services;
}

// In-Memory (for testing)
public static IServiceCollection AddInMemoryData(
    this IServiceCollection services)
{
    services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
    services.AddSingleton<IProviderRepository, InMemoryProviderRepository>();

    return services;
}
```

## Configuration Management

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=bellman;Username=user;Password=password;Pooling=true;"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "bellman_exchange",
    "VirtualHost": "/",
    "Port": 5672
  },
  "ElasticApm": {
    "ServiceName": "Bellman",
    "LogLevel": "Debug",
    "ServerUrl": "http://localhost:8200",
    "TransactionSampleRate": 1.0,
    "SpanFramesMinDuration": "0ms",
    "CaptureBody": "all",
    "CaptureHeaders": true
  },
  "Kundera": {
    "BaseUrl": "http://localhost:1002",
    "ServiceSecret": "your-service-secret-here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "DotNetCore.CAP": "Warning"
    }
  }
}
```

## Event Processing with CAP

### Publisher Configuration

```csharp
builder.Services.AddCap(options =>
{
    options.FailedRetryCount = 1;
    options.FailedRetryInterval = 10;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
    options.SucceedMessageExpiredAfter = 30;
    options.FailedMessageExpiredAfter = 30;

    options.UseRabbitMQ(op =>
    {
        op.HostName = builder.Configuration.GetValue<string>("RabbitMQ:HostName");
        op.UserName = builder.Configuration.GetValue<string>("RabbitMQ:UserName");
        op.Password = builder.Configuration.GetValue<string>("RabbitMQ:Password");
        op.ExchangeName = builder.Configuration.GetValue<string>("RabbitMQ:ExchangeName");
    });

    options.UsePostgreSql(sqlOptions =>
    {
        sqlOptions.ConnectionString = builder.Configuration.GetConnectionString("Default");
        sqlOptions.Schema = "cap";
    });
});
```

### Event Consumer Pattern

```csharp
[CapSubscribe("bellman.notification.send.sms.magfa")]
public class MagfaNotificationConsumer : ICapSubscribe
{
    private readonly AbstractNotificationManagement _magfaProvider;

    public MagfaNotificationConsumer(AbstractNotificationManagement magfaProvider)
        => _magfaProvider = magfaProvider;

    [CapSubscribe("bellman.notification.send.sms.magfa")]
    public async Task HandleSendNotification(SendNotificationEvent @event)
    {
        var request = new SendNotificationRequest
        {
            Id = @event.RequestId,
            Content = @event.Content,
            To = @event.To
        };

        await _magfaProvider.SendAsync(request);
    }
}
```

## Monitoring and Observability

### Elastic APM Integration

```csharp
// Automatic instrumentation for:
- ASP.NET Core requests
- Database calls (EF Core, ADO.NET)
- HTTP client requests
- RabbitMQ messaging
- Custom spans for business logic

// Custom spans example
using var span = _tracer.StartTransaction("notification.send", "notification");
span.SetLabel("provider", providerName);
span.SetLabel("recipient_count", recipients.Length);
try
{
    // business logic
    span.SetOutcome(Outcome.Success);
}
catch (Exception ex)
{
    span.CaptureException(ex);
    span.SetOutcome(Outcome.Failure);
    throw;
}
```

## Testing Strategy

### Unit Tests

```csharp
public class NotificationServiceTests
{
    [Fact]
    public async Task SendAsync_WithValidProvider_PublishesEvent()
    {
        // Arrange
        var mockPublisher = new Mock<ICapPublisher>();
        var mockProviderService = new Mock<IProviderSelectionService>();
        mockProviderService
            .Setup(x => x.ResolveByTypeAsync("sms", default))
            .ReturnsAsync(new Provider { Name = "test", Type = "sms", Status = ProviderStatus.Enable });

        var service = new NotificationService(mockProviderService.Object, mockPublisher.Object);

        // Act
        await service.SendAsync(new SendNotification("test", new[] { "123" }, "sms"), default);

        // Assert
        mockPublisher.Verify(x =>
            x.PublishAsync(It.IsAny<string>(), It.IsAny<SendNotificationEvent>(), default),
            Times.Once);
    }
}
```

### Integration Tests

```csharp
public class NotificationEndpointTests : AppFixture<Program>
{
    [Fact]
    public async Task SendNotification_ReturnsOk()
    {
        // Arrange
        var request = new SendNotification("Hello", new[] { "+1234567890" }, "sms");

        // Act
        var response = await Client.POSTAsync("/api/v1/notifications/send", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## Performance Optimization

### Connection Pooling

```csharp
// PostgreSQL connection pooling
"ConnectionString": "Host=localhost;Port=5432;Database=bellman;Username=user;Password=password;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=20;"

// HTTP client factory for provider APIs
builder.Services.AddHttpClient("MagfaProvider", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "Bellman/1.0");
});
```

### Caching Strategy

```csharp
// Provider cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IProviderCache, ProviderCache>();

public class ProviderCache : IProviderCache
{
    private readonly IMemoryCache _cache;
    private readonly IProviderRepository _repository;

    public async Task<Provider?> GetByNameAsync(string name)
    {
        return await _cache.GetOrCreateAsync(
            $"provider:{name}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _repository.FindByNameAsync(name);
            });
    }
}
```

## Deployment Configuration

### Docker Multi-Stage Build

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5234

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["./." , "bellman/"]
RUN dotnet restore "bellman/Application/Application.csproj"
RUN dotnet build "bellman/Application/Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bellman/Application/Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Application.dll"]
```

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: bellman
spec:
  replicas: 3
  selector:
    matchLabels:
      app: bellman
  template:
    metadata:
      labels:
        app: bellman
    spec:
      containers:
      - name: bellman
        image: bellman:latest
        ports:
        - containerPort: 5234
        env:
        - name: ConnectionStrings__Default
          valueFrom:
            secretKeyRef:
              name: bellman-secrets
              key: database-connection
        - name: RabbitMQ__HostName
          value: "rabbitmq-service"
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "512Mi"
            cpu: "500m"
```

## API Specification

### REST Endpoints

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|-------------|----------|
| POST | `/api/v1/notifications/send` | Send notification | `SendNotification` | `200 OK` |
| GET | `/api/v1/providers/list` | List providers | - | `ProviderResponse[]` |
| GET | `/api/v1/providers/detail` | Get provider details | Query params | `ProviderResponse` |
| PUT | `/api/v1/providers/toggle` | Enable/disable provider | `ToggleProviderRequest` | `200 OK` |

### WebSocket Support (Future)

```csharp
// Real-time notification status updates
app.UseWebSockets();
app.Map("/ws/notifications", async context =>
{
    // WebSocket handler for real-time updates
});
```

## Security Considerations

### Authentication & Authorization

- JWT Bearer tokens via KunderaNet
- Role-based access control for admin endpoints
- API key authentication for provider endpoints
- Request rate limiting

### Data Protection

- Sensitive configuration encrypted
- PII data masked in logs
- Database connection strings secured
- Provider credentials stored in secure vaults

### Network Security

- HTTPS enforced in production
- CORS policy configuration
- IP whitelisting for admin endpoints
- API versioning for backward compatibility

## Troubleshooting Guide

### Common Issues

1. **CAP Connection Issues**
   ```
   Error: RabbitMQ connection failed
   Solution: Check RabbitMQ service status and connection string
   ```

2. **Provider API Timeouts**
   ```
   Error: Provider request timeout
   Solution: Increase HttpClient timeout or implement retry policy
   ```

3. **Database Connection Pool Exhaustion**
   ```
   Error: Connection pool limit reached
   Solution: Increase pool size or implement connection reuse
   ```

### Debug Logging

```csharp
// Enable detailed CAP logging
"Logging": {
  "LogLevel": {
    "DotNetCore.CAP": "Debug"
  }
}

// Enable EF Core query logging
options.EnableSensitiveDataLogging();
options.EnableDetailedErrors();
```

## Contributing Guidelines

### Code Standards

- Follow C# coding conventions
- Use async/await for all I/O operations
- Implement proper exception handling
- Add XML documentation comments
- Write unit tests for new features

### Pull Request Process

1. Create feature branch from `main`
2. Implement changes with tests
3. Update documentation
4. Submit PR with description
5. Code review and approval
6. Merge after CI passes

---

This technical documentation provides comprehensive details about Bellman's architecture, implementation, and development practices. For user-facing information, see the main [README](../Readme.MD).
