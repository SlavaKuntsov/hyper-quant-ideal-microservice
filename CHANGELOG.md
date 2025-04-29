# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## Future Versions

- Multi-exchange support:
  - Extend beyond current Binance implementations (Spot, USDT Futures, Coin Futures)
  - Add support for additional cryptocurrency exchanges

- Enhanced Job System:
  - Improve job scheduling capabilities
  - Support for multiple job extensions
  - Better job monitoring and management

- Advanced Notification System:
  - Dynamic notification configuration
  - Flexible recipients management (add, remove recipients without restarting service)

- Endpoints
  - Update `SymbolController` endpoints to use optional query parameters in routes instead of long hardcoded routes

## [0.6.0] — 2025-04-22

### Added
- Functional tests for all endpoints.
- Documentation

## [0.5.0] — 2025-04-22

### Added
- Notification system:
  - Standard notifications via Telegram and email.
  - Critical (emergency) notifications via email.

## [0.4.0] — 2025-04-21

### Added
- Controllers and request models.
- Global middleware for exception handling.
- Background jobs system:
  - Base extensible job implementation.
  - Jobs for Binance Spot, Binance USDT Futures, and Binance Coin Futures.

## [0.3.0] — 2025-04-21

### Added
- Application layer:
  - Interfaces for services.
  - Service implementations.
  - Dtos, mappings

## [0.2.0] — 2025-04-11

### Added
- Domain models.
- Data access layer:
  - `ApplicationDbContext`.
  - `ApplicationDbContextDesignTimeFactory`.
  - Repositories and their interfaces.
  - Unit of Work.

## [0.1.0] — 2025-04-07

### Added
- Basic project structure.
- Logging configured via Serilog.
- Docker and Docker Compose support.
