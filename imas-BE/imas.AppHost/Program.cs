var builder = DistributedApplication.CreateBuilder(args);

// Use existing SQL Server connection - no Docker required
// All services will use the same database with different schemas/table prefixes

// Add all microservices with specific ports
var generalService = builder.AddProject<Projects.imas_General_ApiService>("general-service")
    .WithHttpsEndpoint(port: 7201, name: "general-https")
    .WithHttpEndpoint(port: 7301, name: "general-http");

var hrService = builder.AddProject<Projects.imas_HR_ApiService>("hr-service")
    .WithHttpsEndpoint(port: 7202, name: "hr-https")
    .WithHttpEndpoint(port: 7302, name: "hr-http");

var assetsService = builder.AddProject<Projects.imas_Assets_ApiService>("assets-service")
    .WithHttpsEndpoint(port: 7203, name: "assets-https")
    .WithHttpEndpoint(port: 7303, name: "assets-http");

var financeService = builder.AddProject<Projects.imas_Finance_ApiService>("finance-service")
    .WithHttpsEndpoint(port: 7204, name: "finance-https")
    .WithHttpEndpoint(port: 7304, name: "finance-http");

// Main API Gateway service
var apiService = builder.AddProject<Projects.imas_ApiService>("api-gateway")
    .WithHttpsEndpoint(port: 7200, name: "api-https")
    .WithHttpEndpoint(port: 7300, name: "api-http")
    .WithReference(generalService)
    .WithReference(hrService)
    .WithReference(assetsService)
    .WithReference(financeService);

// Web frontend (Blazor WebAssembly)
builder.AddProject<Projects.imas_web3>("web3-frontend")
    .WithHttpsEndpoint(port: 7206, name: "web3-https")
    .WithHttpEndpoint(port: 7306, name: "web3-http")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
