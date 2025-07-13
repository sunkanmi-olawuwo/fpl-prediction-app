using System.Text.Json.Serialization;
using System.Text.Json;

namespace FPlPredictionApp.ApiService;

public class FootballDataApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _fplApiUrl;
    private readonly string _fplFixturesUrl;
    private readonly string _fplEventLiveUrl;

    public FootballDataApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        
        // Get URLs from configuration or use defaults
        // Try both direct indexer and GetValue to be compatible with different mocking approaches
        _fplApiUrl = configuration["FPL_API_URL"] ?? 
                     configuration.GetSection("FPL_API_URL")?.Value ?? 
                     "https://fantasy.premierleague.com/api/bootstrap-static/";
                     
        _fplFixturesUrl = configuration["FPL_FIXTURES_URL"] ?? 
                          configuration.GetSection("FPL_FIXTURES_URL")?.Value ?? 
                          "https://fantasy.premierleague.com/api/fixtures/";
                          
        _fplEventLiveUrl = configuration["FPL_EVENT_LIVE_URL"] ?? 
                           configuration.GetSection("FPL_EVENT_LIVE_URL")?.Value ?? 
                           "https://fantasy.premierleague.com/api/event/{0}/live/";
    }

    public async Task<IReadOnlyList<FplTeam>> GetPremierLeagueTeamsAsync()
    {
        var response = await _httpClient.GetAsync(_fplApiUrl);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FplBootstrapResponse>();
        return result!.Teams;
    }

    public async Task<FplBootstrapResponse> GetPremierLeagueDataAsync()
    {
        var response = await _httpClient.GetAsync(_fplApiUrl);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FplBootstrapResponse>();
        return result ?? new FplBootstrapResponse();
    }

    public async Task<List<FplFixture>> GetPremierLeagueFixturesAsync()
    {
        var response = await _httpClient.GetAsync(_fplFixturesUrl);
        response.EnsureSuccessStatusCode();
        var fixtures = await response.Content.ReadFromJsonAsync<List<FplFixture>>();
        return fixtures ?? new List<FplFixture>();
    }

    public async Task<FplEventLiveResponse?> GetEventLiveAsync(int eventId)
    {
        var url = string.Format(_fplEventLiveUrl, eventId);
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FplEventLiveResponse>();
    }

    public async Task<List<FplEventLivePlayer>> FetchAndStoreHistoricalPlayerDataAsync(int startEventId, int endEventId, string filePath)
    {
        var allPlayers = new List<FplEventLivePlayer>();
        for (int eventId = startEventId; eventId <= endEventId; eventId++)
        {
            var eventLive = await GetEventLiveAsync(eventId);
            if (eventLive?.Elements != null)
            {
                foreach (var player in eventLive.Elements)
                {
                    player.EventId = eventId;
                    allPlayers.Add(player);
                }
            }
        }
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(allPlayers));
        return allPlayers;
    }
}

public class FplBootstrapResponse
{
    [JsonPropertyName("elements")]
    public List<FplPlayerStats> Elements { get; set; } = new();
    [JsonPropertyName("teams")]
    public List<FplTeam> Teams { get; set; } = new();
    [JsonPropertyName("events")]
    public List<FplEvent> Events { get; set; } = new();
}

public class FplPlayerStats
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;
    [JsonPropertyName("second_name")]
    public string SecondName { get; set; } = string.Empty;
    [JsonPropertyName("web_name")]
    public string WebName { get; set; } = string.Empty;
    [JsonPropertyName("team")]
    public int TeamId { get; set; }
    [JsonPropertyName("element_type")]
    public int PositionId { get; set; }
    [JsonPropertyName("total_points")]
    public int TotalPoints { get; set; }
    [JsonPropertyName("goals_scored")]
    public int Goals { get; set; }
    [JsonPropertyName("assists")]
    public int Assists { get; set; }
    [JsonPropertyName("minutes")]
    public int MinutesPlayed { get; set; }
}

public class FplTeam
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("short_name")]
    public string ShortName { get; set; } = string.Empty;
    [JsonPropertyName("strength")]
    public int Strength { get; set; }
}

public class FplEvent
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("deadline_time")]
    public string DeadlineTime { get; set; } = string.Empty;
    // Add more event/gameweek properties as needed
}

public class FplFixture
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("event")]
    public int EventId { get; set; }
    [JsonPropertyName("team_h")]
    public int HomeTeamId { get; set; }
    [JsonPropertyName("team_a")]
    public int AwayTeamId { get; set; }
    [JsonPropertyName("team_h_score")]
    public int? HomeTeamScore { get; set; }
    [JsonPropertyName("team_a_score")]
    public int? AwayTeamScore { get; set; }
    // Add more fixture properties as needed
}

public class FplEventLiveResponse
{
    [JsonPropertyName("elements")]
    public List<FplEventLivePlayer> Elements { get; set; } = new();
}

public class FplEventLivePlayer
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("stats")]
    public FplEventLiveStats Stats { get; set; } = new();
    [JsonIgnore]
    public int EventId { get; set; } // Added for tracking
}

public class FplEventLiveStats
{
    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }
    [JsonPropertyName("goals_scored")]
    public int Goals { get; set; }
    [JsonPropertyName("assists")]
    public int Assists { get; set; }
    [JsonPropertyName("clean_sheets")]
    public int CleanSheets { get; set; }
    [JsonPropertyName("total_points")]
    public int TotalPoints { get; set; }
    // Add more stats as needed
}
