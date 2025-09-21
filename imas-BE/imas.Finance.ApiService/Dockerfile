FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["imas.Finance.ApiService/imas.Finance.ApiService.csproj", "imas.Finance.ApiService/"]
COPY ["imas.Shared/imas.Shared.csproj", "imas.Shared/"]
RUN dotnet restore "imas.Finance.ApiService/imas.Finance.ApiService.csproj"
COPY . .
WORKDIR "/src/imas.Finance.ApiService"
RUN dotnet build "imas.Finance.ApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "imas.Finance.ApiService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "imas.Finance.ApiService.dll"]