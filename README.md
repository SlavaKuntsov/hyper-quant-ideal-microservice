# InfoSymbolServer

InfoSymbolServer is a centralized system for maintaining and providing trading instrument data from various exchanges. The primary purpose of this service is to reliably track changes in available trading instruments and promptly notify administrators about these changes.

## Supported Exchanges

Currently, InfoSymbolServer supports the following exchanges:

- **BinanceSpot**
- **BinanceCoinFutures**
- **BinanceUsdtFutures**

Support for additional exchanges will be implemented in future versions.

## How It Works

The system operates on a scheduled basis, performing the following steps:

1. Fetch current instrument data from exchange APIs (currently Binance is implemented)
2. Compare retrieved data with information stored in the database
3. Identify changes (additions, removals, modifications)
4. Update the database to reflect the current state
5. Send notifications to administrators when changes are detected

## Project Structure

The project follows Clean Architecture principles with the following layers:

- **Domain**: Core business entities and repository interfaces
- **Application**: Application services, DTOs, and business logic
- **Infrastructure**: Data access, external API clients, and background jobs
- **Presentation**: Web API endpoints and HTTP request handling
- **InfoSymbolServer**: Startup project

## Configuration

InfoSymbolServer uses the standard .NET configuration system with `appsettings.json` files. Key configuration areas include:

### Logging (Serilog)

Logging is configured via the `Serilog` section. Example:
```json
"Serilog": {
  "Using": [ "Serilog.Sinks.Console" ],
  "MinimumLevel": { "Default": "Information" },
  "WriteTo": [
    { "Name": "Console", "Args": { "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}" } }
  ],
  "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
}
```

### Database Connection

Set in the `ConnectionStrings` section:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=db;Database=infosymboldb;Username=postgres;Password=postgres;IncludeErrorDetail=true"
}
```

### Background Jobs

Scheduled using Quartz.NET via the `BackgroundJobs` section:
```json
"BackgroundJobs": {
  "BinanceSymbolSyncJobSchedule": "0 0/3 * * * ?"
}
```

### Notifications

Configure Telegram and Email notifications in the `Notifications` section:
```json
"Notifications": {
  "Telegram": {
    "BotToken": "",
    "ChatIds": [ "" ],
    "IncludeDetailedInfo": true,
    "MaxSymbolsPerMessage": 5
  },
  "Email": {
    "SmtpServer": "",
    "SmtpPort": 587,
    "UseSsl": true,
    "SenderEmail": "",
    "SenderName": "",
    "Username": "",
    "Password": "",
    "Recipients": [ "" ]
  }
}
```

---

## Deployment

InfoSymbolServer is containerized using Docker and can be deployed using Docker Compose for development and testing environments.

### Quick Start

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/infosymbolserver.git
   cd infosymbolserver
   ```
2. Start the services:
   ```bash
   docker-compose up -d
   ```
3. Verify the deployment:
   ```bash
   docker-compose ps
   ```
4. Check the logs:
   ```bash
   docker-compose logs -f infosymbolserver
   ```
5. Access the API:
   - Swagger UI: http://localhost:61578/swagger
   - API Endpoints: http://localhost:61578/api/v1/...

To stop the services but keep the data volumes:
```bash
docker-compose down
```
To stop the services and remove the data volumes:
```bash
docker-compose down -v
```

### Docker Compose Overview

The `docker-compose.yml` file defines two main services:
- **infosymbolserver**: The main application service (exposes port 61578)
- **db**: PostgreSQL database service (exposes port 61579, persists data with a named volume)

For more details, see project wiki documentation.