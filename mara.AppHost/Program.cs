var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.mara_ApiService>("apiservice");

builder.AddProject<Projects.mara_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
