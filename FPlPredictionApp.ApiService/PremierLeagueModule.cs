using Carter;

namespace FPlPredictionApp.ApiService;

public class PremierLeagueModule : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/premierleague/data", async (HttpContext ctx) =>
        {
            var client = ctx.RequestServices.GetRequiredService<FootballDataApiClient>();
            var data = await client.GetPremierLeagueDataAsync();
            await ctx.Response.WriteAsJsonAsync(data);
        });

        app.MapGet("/premierleague/teams", async (HttpContext ctx) =>
        {
            var client = ctx.RequestServices.GetRequiredService<FootballDataApiClient>();
            var data = await client.GetPremierLeagueTeamsAsync();
            await ctx.Response.WriteAsJsonAsync(data);
        });

        app.MapGet("/premierleague/fixtures", async (HttpContext ctx) =>
        {
            var client = ctx.RequestServices.GetRequiredService<FootballDataApiClient>();
            var fixtures = await client.GetPremierLeagueFixturesAsync();
            await ctx.Response.WriteAsJsonAsync(fixtures);
        });
    }
}
