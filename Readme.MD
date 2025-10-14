# 🔔 Bellman — Smart Notification Service Dispatcher

Bellman is an event‑driven gateway for sending SMS and email notifications. You integrate once with Bellman and it handles provider selection, retries, and delivery tracking across multiple providers.

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
  - [Local](#local)
  - [Docker](#docker)
- [Configuration](#configuration)
- [Usage](#usage)
- [Documentation](#documentation)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

## Overview
Bellman exposes a simple HTTP API. Internally it publishes events (RabbitMQ + CAP) and dispatches to provider implementations.

Supported providers today:
- SMS: Magfa, Persiafava, Ssmss
- Email: Tes

See [docs/TECHNICAL.md](docs/TECHNICAL.md) for architecture and extension points.

## Features
- Smart provider resolution (by type or explicit name)
- Automatic retries (configurable)
- Event‑driven processing via RabbitMQ and PostgreSQL event store
- Observable by design (Elastic APM integration)
- Simple REST API with Swagger UI

## Prerequisites
- .NET 9 SDK (for local development)
- PostgreSQL 15+
- RabbitMQ 3.x

## Quick Start

### Local
```bash
# 1) Start dependencies (Linux/macOS/Windows with Docker)
docker run -d --name bellman-postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:15
docker run -d --name bellman-rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# 2) Configure (edit Application/appsettings.Development.json if needed)
#    Ensure the connection string name is "default" (lowercase)

# 3) Run the API
dotnet run --project Application
```

Your API will be available at `http://localhost:5234` with Swagger UI at `/swagger`.

### Docker
```bash
# Build image
docker build -t bellman .

# Run container (configure via env vars)
docker run --rm -p 5234:5234 \
  -e ConnectionStrings__default="Host=localhost;Port=5432;Database=bellman;Username=user;Password=password" \
  -e RabbitMQ__HostName="localhost" \
  -e RabbitMQ__UserName="guest" \
  -e RabbitMQ__Password="guest" \
  -e RabbitMQ__ExchangeName="bellman_exchange" \
  bellman
```

## Configuration
Bellman uses standard .NET configuration (JSON files, environment variables, etc.). Minimal settings:

```json
{
  "ConnectionStrings": {
    "default": "Host=localhost;Port=5432;Database=bellman;Username=user;Password=password"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "bellman_exchange"
  }
}
```

Environment variable overrides (examples):
- `ConnectionStrings__default`
- `RabbitMQ__HostName`, `RabbitMQ__UserName`, `RabbitMQ__Password`, `RabbitMQ__ExchangeName`

For Elastic APM and other options, see [docs/TECHNICAL.md](docs/TECHNICAL.md).

## Usage
Send your first notification:

```bash
curl -X POST "http://localhost:5234/api/v1/notifications/send" \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Hello from Bellman!",
    "to": ["+1234567890"],
    "type": "sms"
  }'
```

Or open Swagger UI at `http://localhost:5234/swagger`.

## Documentation
- Technical details, architecture, and provider guides: [docs/TECHNICAL.md](docs/TECHNICAL.md)

## Troubleshooting
- RabbitMQ: ensure `RabbitMQ:ExchangeName` is set; connectivity to port 5672 must work
- Connection string: the key must be `ConnectionStrings:default` (lowercase)
- Port binding: ensure port `5234` is free on your host

## Contributing
Contributions are welcome! Please:
1. Read [docs/TECHNICAL.md](docs/TECHNICAL.md) for development setup
2. Create a feature branch
3. Add tests where applicable
4. Open a pull request with a clear description

## License
MIT. If missing, add a `LICENSE` file with the MIT license text.
