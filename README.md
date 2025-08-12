# BlackFinch Lending Platform

A .NET console application for loan processing and statistics.

## Running with Docker

Build and run the application using Docker:

```bash
# Build the Docker image
docker build -t lending-platform -f LendingPlatform.ConsoleApp/Dockerfile .

# Run the application interactively
docker run --rm -it lending-platform
```

## Requirements

- Docker

## Project Structure

- `LendingPlatform.ConsoleApp/` - Main console application
- `LendingPlatform.Tests/` - Unit tests

## Running Tests

To run tests locally (requires .NET 9.0 SDK):

```bash
dotnet test
```
