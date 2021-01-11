using Goals.Producer.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;


namespace Goals.Producer.Repositories
{
    public class FootballDataSource
    {
        readonly Uri _baseAddress = new Uri("http://api.football-data.org/v2/");

        readonly ILogger<FootballDataSource> _logger;

        readonly Dictionary<int, FootballMatch> _matches = new();

        readonly HttpClient _httpClient = new();

        readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };



        public FootballDataSource(ILogger<FootballDataSource> logger, FootballApiConfig apiConfig)
        {
            _logger = logger;

            _logger.LogInformation("Configuring http client");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", apiConfig.ApiKey);
            _httpClient.BaseAddress = _baseAddress;

            RefreshIntervalInSeconds = apiConfig.RefreshIntervalInSeconds;
        }


        public int RefreshIntervalInSeconds { get; init; }


        public async IAsyncEnumerable<FootballMatch> GetMatchUpdates()
        {
            var matches = await GetRawContent();
            foreach(var match in matches.Matches)
            {
                if(IsNewMatch(match))
                {
                    _logger.LogInformation($"New match: {match.HomeTeam.Name} Vs {match.AwayTeam.Name} ({match.Id})");
                    _matches.Add(match.Id, match);
                    yield return match;
                }
                else
                {
                    if(IsUpdatedMatch(match))
                    {
                        _logger.LogInformation
                        (
                            string.Format
                            (
                                "Matched updated: {0} Vs {1}: {2}-{3} {4}",
                                match.HomeTeam.Name,
                                match.AwayTeam.Name,
                                match.Score.LiveScore.HomeTeam,
                                match.Score.LiveScore.AwayTeam,
                                match.Status
                            )
                        );
                        _matches[match.Id] = match;
                        yield return match;
                    }
                }
            }


            async Task<FootballMatches> GetRawContent() =>
                await JsonSerializer.DeserializeAsync<FootballMatches>
                (
                    // TODO: hard coded test dates
                    await _httpClient.GetStreamAsync("matches?competitions=2021&dateFrom=2021-01-02&dateTo=2021-01-04&status=LIVE,IN_PLAY,FINISHED"),
                    _jsonSerializerOptions
                )
            ;

            bool IsNewMatch(FootballMatch match) => ! _matches.ContainsKey(match.Id);

            bool IsUpdatedMatch(FootballMatch match) =>
                   match.Score.LiveScore.Equals(_matches[match.Id].Score.LiveScore) == false
                || match.Status != _matches[match.Id].Status
            ;
        }
    }
}
