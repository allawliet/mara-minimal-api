FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["imas.Web/imas.Web.csproj", "imas.Web/"]
COPY ["imas.ServiceDefaults/imas.ServiceDefaults.csproj", "imas.ServiceDefaults/"]
RUN dotnet restore "imas.Web/imas.Web.csproj"
COPY . .
WORKDIR "/src/imas.Web"
RUN dotnet build "imas.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "imas.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "imas.Web.dll"]