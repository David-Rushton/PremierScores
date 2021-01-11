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
        public async Task Invoke(List<FootballMatch> footballMatches, DateTime dateFrom, DateTime dateUntil)
        {
            var currentDateTime = DateTime.MinValue;
            var teamPoints = new Dictionary<string, int>();
            var teamMatches = new Dictionary<(string team, int matchId), int>();
            var table = GetInitialTable();


            foreach(var match in footballMatches.OrderBy(fbm => fbm.LastUpdated))
            {
                if(match.LastUpdated > dateUntil)
                    throw new Exception("WE SHOULD RETURN HERE");


                foreach(var teamMatch in GetHomeAndAway(match))
                {
                    if( ! teamMatches.ContainsKey(teamMatch.key) )
                        teamMatches.Add(teamMatch.key, teamMatch.points);
                    else
                        teamMatches[teamMatch.key] = teamMatch.points;


                }


                if(match.LastUpdated > currentDateTime && match.LastUpdated > dateFrom)
                {

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


        private Dictionary<string, FootballTableRow> GetInitialTable() =>
            new []
            {
                new FootballTableRow("Arsenal FC", 1),
                new FootballTableRow("Aston Villa FC", 2),
                new FootballTableRow("Brighton & Hove Albion FC", 3),
                new FootballTableRow("Burnley FC", 4),
                new FootballTableRow("Chelsea FC", 5),
                new FootballTableRow("Crystal Palace FC", 6),
                new FootballTableRow("Everton FC", 7),
                new FootballTableRow("Fulham FC", 8),
                new FootballTableRow("Leeds United FC", 9),
                new FootballTableRow("Leicester City FC", 10),
                new FootballTableRow("Liverpool FC", 11),
                new FootballTableRow("Manchester City FC", 12),
                new FootballTableRow("Manchester United FC", 13),
                new FootballTableRow("Newcastle United FC", 14),
                new FootballTableRow("Sheffield United FC", 15),
                new FootballTableRow("Southampton FC", 16),
                new FootballTableRow("Tottenham Hotspur FC", 17),
                new FootballTableRow("West Bromwich Albion FC", 18),
                new FootballTableRow("West Ham United FC", 19),
                new FootballTableRow("Wolverhampton Wanderers FC", 20)
            }.ToDictionary(k => k.Team, v => v)
        ;
    }
}
