# Natron

A lightweight microservice framework for .NET that provides a composable architecture for building services with HTTP, Kafka, and AWS SQS components.

## Overview

Natron provides a simple, component-based approach to building microservices. The framework consists of:

- **Natron.Core** - Base framework with service lifecycle management
- **Natron.Http** - HTTP server component with health checks and observability
- **Natron.Kafka** - Kafka consumer and producer components
- **Natron.AWS** - AWS SQS consumer component

## Key Features

- Component-based architecture allowing multiple concurrent processing components
- Built-in graceful shutdown handling
- HTTP endpoints with Prometheus metrics support
- Configurable error handling strategies for message processing
- Structured logging with Microsoft.Extensions.Logging

## Getting Started

### Installation

Add the required Natron packages to your project:

```bash
dotnet add package Natron
dotnet add package Natron.Http
dotnet add package Natron.Kafka
dotnet add package Natron.AWS
```

### Basic Usage

```csharp
using Microsoft.Extensions.Logging;
using Natron;
using Natron.Http;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var cts = new CancellationTokenSource();

var httpConfig = new Natron.Http.Config();
var httpComponent = new Component(loggerFactory, httpConfig);

using var service = ServiceBuilder.Create("my-service", loggerFactory, cts)
    .ConfigureComponents(httpComponent)
    .Build();

await service.RunAsync();
```

### Example Application

See `src/Natron.Example/Program.cs` for a complete example demonstrating:
- HTTP endpoints with health checks
- Kafka consumer with configurable error handling
- AWS SQS consumer integration

### Kafka Consumer Error Handling

The Kafka consumer supports two error handling strategies:

- **Crash** (default) - Service terminates on processing errors
- **LogAndContinue** - Logs errors and continues processing subsequent messages

```csharp
var config = new Natron.Kafka.Consumer.Config(
    consumerConfig, 
    topics, 
    ProcessingStrategy.LogAndContinue
);
```

## Building

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

For integration tests that require Docker services (Kafka, LocalStack):

```bash
docker-compose up -d
dotnet test
```

## License

See LICENSE file for details.
