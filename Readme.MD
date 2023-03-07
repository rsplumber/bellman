# Bellman

## Notification Service Dispatcher

This document describes an smart, dynamic and event driven Bellman for dispatching notifications between multiple providers with support of multiple types of notifications like SMS, Email and push notifications. This service also supports retry mechanism and logs every sent and failed requests.

## Overview

The Bellman is a microservice that acts as a mediator between different notification providers and consumers. It exposes a RESTful API for sending notifications to various recipients using different channels. It also handles the logic of choosing the best provider for each notification based on some criteria such as cost, availability, performance, etc.

The Bellman uses Kafka as a message broker to decouple the communication between providers and consumers. It also uses PostgreSQL as a primary database to store the configuration and status of each provider and notification and as an event sourcing database to capture all the events related to notifications such as sent, failed, retried, etc.

The Bellman is designed to be cloud native and docker friendly. It can be deployed on any cloud platform that supports Docker containers. It also provides a Dockerfile and a docker-compose file for local development and testing.

## Architecture

The following diagram shows the high-level architecture of the Bellman:

![Notification Service Dispatcher Architecture](notification-service-dispatcher-architecture.png)

The main components of the Bellman are:

- **API Gateway**: This is the entry point for all requests from consumers. It validates and authenticates the requests, then forwards them to the appropriate controller.
- **Controller**: This is responsible for processing the requests from API Gateway and sending them to Kafka topics based on their type (SMS, Email or Push). It also returns an acknowledgement response to API Gateway with a unique identifier for each notification.
- **Dispatcher**: This is responsible for consuming messages from Kafka topics and dispatching them to different providers based on some logic. It also updates PostgreSQL database with the status of each notification (sent or failed) and publishes events to EventStore database.
- **Provider**: This is an abstract interface that defines how to send notifications using different channels (SMS, Email or Push). Each provider implements this interface according to its own specifications and requirements.
- **Retry Service**: This is responsible for retrying failed notifications based on some policy (e.g., number of attempts, time interval, etc.). It consumes events from EventStore database and sends them back to Kafka topics for re-dispatching.
- **Logger Service**: This is responsible for logging all events related to notifications such as sent, failed, retried, etc. It consumes events from EventStore database and writes them to a log file or any other destination.

## How To Use

To use the Bellman, you need to follow these steps:
### Docker
1. Clone repository: `git clone */notification-service-dispatcher.git`
2. Run `docker run --name some-postgres -e POSTGRES_PASSWORD=mysecretpassword -d postgres` (add your credentials to appsettings.json)
3. Install kafka docker using this link: https://developer.confluent.io/quickstart/kafka-docker/ 
4. Run `docker run --name bellman --ports "5111:5111"` 
### Custom

1. Clone repository: `git clone https://github.com/notification-service-dispatcher.git`
2. Install DotNet 7 SDK: `https://dotnet.microsoft.com/download/dotnet/7`
3. Install PostgreSQL: `https://www.postgresql.org/download`
4. Install Kafka: `https://kafka.apache.org/downloads`
5. Run `docker run --name bellman --ports "5111:5111"`

## How To Develop

To develop new features or fix bugs in the Bellman,
you need to follow these steps:
1. Implement `AbstractNotificationManagement`
2. Register with `services.AddProvider<TManagmenet>()`
3. All done
