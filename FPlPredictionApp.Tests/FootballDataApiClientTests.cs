using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Moq.Protected;
using FPlPredictionApp.ApiService;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace FPlPredictionApp.Tests;

public class FootballDataApiClientTests
{
    private IConfiguration CreateMockConfiguration()
    {
        // Create configuration sections for each key
        var mockApiUrlSection = new Mock<IConfigurationSection>();
        mockApiUrlSection.Setup(s => s.Value).Returns("https://fantasy.premierleague.com/api/bootstrap-static/");
        
        var mockFixturesUrlSection = new Mock<IConfigurationSection>();
        mockFixturesUrlSection.Setup(s => s.Value).Returns("https://fantasy.premierleague.com/api/fixtures/");
        
        var mockEventLiveUrlSection = new Mock<IConfigurationSection>();
        mockEventLiveUrlSection.Setup(s => s.Value).Returns("https://fantasy.premierleague.com/api/event/{0}/live/");

        // Create root configuration
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["FPL_API_URL"]).Returns("https://fantasy.premierleague.com/api/bootstrap-static/");
        mockConfig.Setup(c => c["FPL_FIXTURES_URL"]).Returns("https://fantasy.premierleague.com/api/fixtures/");
        mockConfig.Setup(c => c["FPL_EVENT_LIVE_URL"]).Returns("https://fantasy.premierleague.com/api/event/{0}/live/");
        
        // Setup GetSection method to return the appropriate sections
        mockConfig.Setup(c => c.GetSection("FPL_API_URL")).Returns(mockApiUrlSection.Object);
        mockConfig.Setup(c => c.GetSection("FPL_FIXTURES_URL")).Returns(mockFixturesUrlSection.Object);
        mockConfig.Setup(c => c.GetSection("FPL_EVENT_LIVE_URL")).Returns(mockEventLiveUrlSection.Object);

        return mockConfig.Object;
    }

    [Test]
    public async Task GetPremierLeagueDataAsync_ReturnsData()
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        var sampleJson = JsonSerializer.Serialize(new FplBootstrapResponse { Elements = new List<FplPlayerStats> { new FplPlayerStats { Id = 1, FirstName = "Test", SecondName = "Player" } }, Teams = new List<FplTeam>(), Events = new List<FplEvent>() });
        mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(sampleJson) });
        var client = new HttpClient(mockHandler.Object);
        var apiClient = new FootballDataApiClient(client, CreateMockConfiguration());
        var result = await apiClient.GetPremierLeagueDataAsync();
        Assert.That(result.Elements.Count, Is.EqualTo(1));
        Assert.That(result.Elements[0].FirstName, Is.EqualTo("Test"));
    }

    [Test]
    public async Task GetPremierLeagueFixturesAsync_ReturnsFixtures()
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        var sampleJson = JsonSerializer.Serialize(new List<FplFixture> { new FplFixture { Id = 1, EventId = 1, HomeTeamId = 1, AwayTeamId = 2 } });
        mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(sampleJson) });
        var client = new HttpClient(mockHandler.Object);
        var apiClient = new FootballDataApiClient(client, CreateMockConfiguration());
        var result = await apiClient.GetPremierLeagueFixturesAsync();
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].HomeTeamId, Is.EqualTo(1));
    }

    [Test]
    public async Task GetEventLiveAsync_ReturnsEventLiveData()
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        var sampleJson = JsonSerializer.Serialize(new FplEventLiveResponse { Elements = new List<FplEventLivePlayer> { new FplEventLivePlayer { Id = 1, Stats = new FplEventLiveStats { Goals = 2 } } } });
        mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(sampleJson) });
        var client = new HttpClient(mockHandler.Object);
        var apiClient = new FootballDataApiClient(client, CreateMockConfiguration());
        var result = await apiClient.GetEventLiveAsync(1);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Elements.Count, Is.EqualTo(1));
        Assert.That(result.Elements[0].Stats.Goals, Is.EqualTo(2));
    }
}
