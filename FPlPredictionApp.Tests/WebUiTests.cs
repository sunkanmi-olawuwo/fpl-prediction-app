using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Aspire.Hosting.Testing;

namespace FPlPredictionApp.Tests;

public class WebUiTests
{
    [Test]
    public async Task PlayersPageRendersSuccessfully()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FPlPredictionApp_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();
        var httpClient = app.CreateHttpClient("webfrontend");
        var response = await httpClient.GetAsync("/players");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("Premier League Players"));
        Assert.That(content, Does.Contain("table"));
    }

    [Test]
    public async Task TeamsPageRendersSuccessfully()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FPlPredictionApp_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();
        var httpClient = app.CreateHttpClient("webfrontend");
        var response = await httpClient.GetAsync("/teams");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("Premier League Teams"));
        Assert.That(content, Does.Contain("card"));
    }

    [Test]
    public async Task FixturesPageRendersSuccessfully()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FPlPredictionApp_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();
        var httpClient = app.CreateHttpClient("webfrontend");
        var response = await httpClient.GetAsync("/fixtures");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("Premier League Fixtures"));
        Assert.That(content, Does.Contain("table"));
    }
}
