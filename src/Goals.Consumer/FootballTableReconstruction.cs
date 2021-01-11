using Goals.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Goals.Consumer
{
    public class FootballTableReconstruction
    {
        public IEnumerable<FootballTable> GetFootballTables(List<FootballMatch> footballMatches, DateTime dateFrom, DateTime dateUntil)
        {
            var currentDateTime = DateTime.MinValue;
            var teamPoints = new Dictionary<string, int>();
            var teamMatches = new Dictionary<(string team, int matchId), int>();


            foreach(var match in footballMatches.OrderBy(fbm => fbm.LastUpdated))
            {
                if(match.LastUpdated > dateUntil)
                    break;


                foreach(var teamMatch in GetHomeAndAway(match))
                    if( ! teamMatches.ContainsKey(teamMatch.key) )
                        teamMatches.Add(teamMatch.key, teamMatch.points);
                    else
                        teamMatches[teamMatch.key] = teamMatch.points;


                if(match.LastUpdated > currentDateTime && match.LastUpdated > dateFrom)
                {
                    var table = new FootballTable();
                    foreach(var result in teamMatches)
                        table.AddResult(match.LastUpdated, result.Key.team, result.Value);

                    yield return table;
                }


                currentDateTime = match.LastUpdated;
            }


            IEnumerable<((string team, int id) key, int points)> GetHomeAndAway(FootballMatch match)
            {
                yield return ((match.HomeTeam.Name, match.Id), GetMatchPoints(match.Score.LiveScore.HomeTeam, match.Score.LiveScore.AwayTeam));
                yield return ((match.AwayTeam.Name, match.Id), GetMatchPoints(match.Score.LiveScore.AwayTeam, match.Score.LiveScore.HomeTeam));
            }

            int GetMatchPoints(int? teamScore = 0, int? opponentScore = 0)
            {
                if(teamScore == opponentScore)
                    return 1;

                if(teamScore > opponentScore)
                    return 3;

                return 0;
            }
        }
    }
}
