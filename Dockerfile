# Multi-stage build for IMAS modules
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files for all projects
COPY ["imas-BE/imas.ApiService/imas.ApiService.csproj", "imas-BE/imas.ApiService/"]
COPY ["imas-BE/imas.Shared/imas.Shared.csproj", "imas-BE/imas.Shared/"]
COPY ["imas-BE/imas.ServiceDefaults/imas.ServiceDefaults.csproj", "imas-BE/imas.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "imas-BE/imas.ApiService/imas.ApiService.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/imas-BE/imas.ApiService"
RUN dotnet build "imas.ApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "imas.ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variable for module mode
ENV ModuleCommunication__Mode=Distributed

ENTRYPOINT ["dotnet", "imas.ApiService.dll"]