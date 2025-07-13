using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Aspire.Hosting.Testing;

namespace FPlPredictionApp.Tests;

public class ApiServiceTests
{
    [Test]
    public async Task GetPremierLeagueDataReturnsOkAndValidContent()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FPlPredictionApp_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice");
        var response = await httpClient.GetAsync("/premierleague/data");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("elements"));
        Assert.That(content, Does.Contain("teams"));
        Assert.That(content, Does.Contain("events"));
    }

    [Test]
    public async Task GetPremierLeagueFixturesReturnsOkAndValidContent()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FPlPredictionApp_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice");
        var response = await httpClient.GetAsync("/premierleague/fixtures");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("event"));
        Assert.That(content, Does.Contain("team_h"));
        Assert.That(content, Does.Contain("team_a"));
    }

    [Test]
    public async Task GetPremierLeagueTeamsReturnsOkAndValidContent()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FPlPredictionApp_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();
        var httpClient = app.CreateHttpClient("apiservice");
        var response = await httpClient.GetAsync("/premierleague/teams");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("id"));
        Assert.That(content, Does.Contain("name"));
        Assert.That(content, Does.Contain("short_name"));
        Assert.That(content, Does.Contain("strength"));
    }
}
