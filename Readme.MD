# 🔔 Bellman - Smart Notification Service Dispatcher

> A modern, event-driven notification service that intelligently dispatches SMS and email notifications across multiple providers with built-in retry mechanisms, real-time monitoring, and comprehensive logging.

## ✨ What Bellman Does

Bellman acts as a smart mediator between your applications and notification providers. Instead of integrating with each SMS/email service individually, you send one request to Bellman and it automatically:

- ✅ Selects the best available provider
- 🔄 Handles retries if delivery fails
- 📊 Tracks delivery status and performance
- 📱 Supports SMS (Magfa, Persiafava, Ssmss) and Email (Tes)
- 🔧 Easily extensible for new providers

## 🚀 Quick Start

### Using Docker (Easiest)

```bash
# 1. Clone and setup
git clone <your-repo-url>
cd bellman

# 2. Start dependencies
docker run --name postgres -e POSTGRES_PASSWORD=mypassword -d postgres:15
docker run --name rabbitmq -p 5672:5672 -p 15672:15672 -d rabbitmq:3-management

# 3. Configure and run
# Edit appsettings.json with your connection strings
dotnet run --project Application
```

**That's it!** Your API is now running at `http://localhost:5234` with interactive docs at `/swagger`.

### Manual Setup

You'll need:
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 15+](https://www.postgresql.org/download)
- [RabbitMQ 3.12+](https://www.rabbitmq.com/download.html)

## 📡 Send Your First Notification

### Via REST API

```bash
curl -X POST "http://localhost:5234/api/v1/notifications/send" \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Hello from Bellman! 🚀",
    "to": ["+989123456789"],
    "type": "sms"
  }'
```

### Via Swagger UI

1. Open `http://localhost:5234/swagger`
2. Find the "Send Notification" endpoint
3. Click "Try it out"
4. Fill in your message details
5. Execute!

## 📦 Supported Providers

| Provider | Type | Region | Status |
|----------|------|--------|--------|
| Magfa | SMS | Iran | ✅ Active |
| Persiafava | SMS | Iran | ✅ Active |
| Ssmss | SMS | Iran | ✅ Active |
| Tes | Email | Global | ✅ Active |

**Need another provider?** Bellman is designed to be easily extensible. See our [technical docs](docs/TECHNICAL.md) for adding new providers.

## 🏗️ Architecture at a Glance

```
Your App → Bellman API → Smart Provider Selection → External Provider APIs
               ↓                    ↓                           ↓
         Event-Driven        Retry Logic &              Delivery Tracking
         Processing         Error Handling            Performance Metrics
```

- **Event-Driven**: Uses RabbitMQ for reliable message processing
- **Fault-Tolerant**: Automatic retries with configurable policies
- **Observable**: Real-time monitoring with Elastic APM
- **Scalable**: Horizontal scaling with Kubernetes support

## 📊 Monitoring & Analytics

Bellman includes comprehensive monitoring out of the box:

- 📈 **Performance Metrics**: Response times, throughput, error rates
- 🔍 **Distributed Tracing**: Track requests across all services
- 📋 **Delivery Reports**: Success/failure status for every notification
- 🎯 **Provider Analytics**: Performance comparison between providers

Access your monitoring dashboard at your Elastic Stack URL.

## 🔧 Configuration

Bellman uses standard .NET configuration. Key settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=bellman;Username=user;Password=password"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  },
  "ElasticApm": {
    "ServiceName": "Bellman",
    "ServerUrl": "http://localhost:8200"
  }
}
```

## 🚀 Production Deployment

### Docker Deployment

```bash
# Build and run
docker build -t bellman .
docker run -p 5234:5234 -e ConnectionStrings__Default="..." bellman
```

### Kubernetes

Bellman is cloud-native and includes Kubernetes manifests for production deployment with auto-scaling, health checks, and rolling updates.

## 📚 Documentation

- **[Technical Documentation](docs/TECHNICAL.md)**: Detailed architecture, API specs, development guides
- **[API Reference](http://localhost:5234/swagger)**: Interactive API documentation (when running)
- **Provider Guides**: Integration details for each supported provider

## 🤝 Contributing

We welcome contributions! Whether you're fixing bugs, adding features, or improving documentation:

1. Check our [technical docs](docs/TECHNICAL.md) for development setup
2. Fork the repository
3. Create a feature branch
4. Make your changes with tests
5. Submit a pull request

## 📄 License

MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋 Support & Community

- 📧 **Issues**: [GitHub Issues](https://github.com/your-repo/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/your-repo/discussions)
- 📖 **Documentation**: [Technical Docs](docs/TECHNICAL.md)

---

**Built with ❤️ using .NET 9, RabbitMQ, and PostgreSQL • [View Technical Details](docs/TECHNICAL.md)**
