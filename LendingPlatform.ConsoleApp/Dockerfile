FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LendingPlatform.ConsoleApp/LendingPlatform.ConsoleApp.csproj", "LendingPlatform.ConsoleApp/"]
RUN dotnet restore "LendingPlatform.ConsoleApp/LendingPlatform.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/LendingPlatform.ConsoleApp"
RUN dotnet build "./LendingPlatform.ConsoleApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LendingPlatform.ConsoleApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LendingPlatform.ConsoleApp.dll"]
