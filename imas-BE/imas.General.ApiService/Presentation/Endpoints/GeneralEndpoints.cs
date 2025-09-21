using imas.General.ApiService.Application;

namespace imas.General.ApiService.Presentation.Endpoints;

public static class GeneralEndpoints
{
    public static void MapGeneralEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/general")
            .WithTags("General")
            .WithOpenApi();

        group.MapGet("/departments", GetDepartments)
             .WithName("GetDepartments")
             .WithSummary("Get all departments");

        group.MapGet("/companies", GetCompanies)
             .WithName("GetCompanies")
             .WithSummary("Get all companies");

        group.MapGet("/locations", GetLocations)
             .WithName("GetLocations")
             .WithSummary("Get all locations");
    }

    private static async Task<IResult> GetDepartments(IGeneralService generalService)
    {
        var result = await generalService.GetDepartmentsAsync();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> GetCompanies(IGeneralService generalService)
    {
        var result = await generalService.GetCompaniesAsync();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> GetLocations(IGeneralService generalService)
    {
        var result = await generalService.GetLocationsAsync();
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}