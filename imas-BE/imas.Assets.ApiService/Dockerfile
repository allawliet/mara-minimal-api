FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["imas.Assets.ApiService/imas.Assets.ApiService.csproj", "imas.Assets.ApiService/"]
COPY ["imas.Shared/imas.Shared.csproj", "imas.Shared/"]
RUN dotnet restore "imas.Assets.ApiService/imas.Assets.ApiService.csproj"
COPY . .
WORKDIR "/src/imas.Assets.ApiService"
RUN dotnet build "imas.Assets.ApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "imas.Assets.ApiService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "imas.Assets.ApiService.dll"]