using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;

namespace FPlPredictionApp.ApiService;

public class FootballDataApiClient(HttpClient httpClient)
{
    private const string FplApiUrl = "https://fantasy.premierleague.com/api/bootstrap-static/";
    private const string FplFixturesUrl = "https://fantasy.premierleague.com/api/fixtures/";
    private const string FplEventLiveUrl = "https://fantasy.premierleague.com/api/event/{0}/live/";

    public async Task<FplBootstrapResponse> GetPremierLeagueDataAsync()
    {
        var response = await httpClient.GetAsync(FplApiUrl);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FplBootstrapResponse>();
        return result ?? new FplBootstrapResponse();
    }

    public async Task<List<FplFixture>> GetPremierLeagueFixturesAsync()
    {
        var response = await httpClient.GetAsync(FplFixturesUrl);
        response.EnsureSuccessStatusCode();
        var fixtures = await response.Content.ReadFromJsonAsync<List<FplFixture>>();
        return fixtures ?? new List<FplFixture>();
    }

    public async Task<FplEventLiveResponse?> GetEventLiveAsync(int eventId)
    {
        var url = string.Format(FplEventLiveUrl, eventId);
        var response = await httpClient.GetAsync(url);
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
    // Add more FPL stats as needed
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
    // Add more team properties as needed
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
