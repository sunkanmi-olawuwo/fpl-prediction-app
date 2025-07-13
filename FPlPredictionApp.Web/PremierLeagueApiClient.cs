namespace FPlPredictionApp.Web;

public class PremierLeagueApiClient(HttpClient httpClient)
{
    public async Task<PremierLeagueDataDto?> GetDataAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<PremierLeagueDataDto>("/premierleague/data", cancellationToken);
    }

    public async Task<List<FixtureDto>> GetFixturesAsync(CancellationToken cancellationToken = default)
    {
        var fixtures = await httpClient.GetFromJsonAsync<List<FixtureDto>>("/premierleague/fixtures", cancellationToken);
        return fixtures ?? new List<FixtureDto>();
    }
}

public class PremierLeagueDataDto
{
    public List<PlayerDto> Elements { get; set; } = new();
    public List<TeamDto> Teams { get; set; } = new();
    public List<EventDto> Events { get; set; } = new();
}

public class PlayerDto
{
    public int Id { get; set; }
    public string WebName { get; set; } = "";
    public int TeamId { get; set; }
    public int PositionId { get; set; }
    public int TotalPoints { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int MinutesPlayed { get; set; }
}

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ShortName { get; set; } = "";
    public int Strength { get; set; }
}

public class EventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string DeadlineTime { get; set; } = "";
}

public class FixtureDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
}
