using System;
using System.Collections.Generic;
using System.Linq;


namespace Goals.Data.Models
{
    public class FootballTable
    {
        readonly Dictionary<string, FootballTableRow> _table;


        public FootballTable()
        {
            LastUpdated = DateTime.MinValue;
            _table = GetInitialTable();
        }


        public DateTime LastUpdated { get; private set; }


        public void AddResult(DateTime updated, string team, int points)
        {
            LastUpdated = updated;
            team = team.Trim();

            switch (points)
            {
                case 0:
                    _table[team].Loses++;
                    break;

                case 1:
                    _table[team].Draws++;
                    break;

                case 3:
                    _table[team].Wins++;
                    break;

                default:
                    throw new Exception("Match points must be equal to 0, 1 or 3");
            }

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            var position = 1;
            var newSequence =
                from team in _table.Values
                orderby team.Points descending, team.Wins descending, team.Team
                select new { team.Team, Position = position++ }
            ;

            foreach(var item in newSequence)
                _table[item.Team].Position = item.Position;
        }

        public string ToJson()
        {
            var orderedTable = _table.OrderBy(ftr => ftr.Value.Position);
            var jsonRows = new List<string>();

            foreach(var row in _table.OrderBy(ftr => ftr.Value.Position))
                jsonRows.Add($"\t\t\t{ row.Value.ToJson() }");


            return string.Format
            (
                GetJsonTemplate(),
                LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"),
                string.Join(",\n", jsonRows)
            );
        }


        // Urg.  Hardcoded.
        private Dictionary<string, FootballTableRow> GetInitialTable() =>
            new []
            {
                new FootballTableRow("Arsenal FC", 1),
                new FootballTableRow("Aston Villa FC", 2),
                new FootballTableRow("Brighton & Hove Albion FC", 3),
                new FootballTableRow("Burnley FC", 4),
                new FootballTableRow("Chelsea FC", 5),
                new FootballTableRow("Crystal Palace FC", 6),
                new FootballTableRow("Everton FC", 7),
                new FootballTableRow("Fulham FC", 8),
                new FootballTableRow("Leeds United FC", 9),
                new FootballTableRow("Leicester City FC", 10),
                new FootballTableRow("Liverpool FC", 11),
                new FootballTableRow("Manchester City FC", 12),
                new FootballTableRow("Manchester United FC", 13),
                new FootballTableRow("Newcastle United FC", 14),
                new FootballTableRow("Sheffield United FC", 15),
                new FootballTableRow("Southampton FC", 16),
                new FootballTableRow("Tottenham Hotspur FC", 17),
                new FootballTableRow("West Bromwich Albion FC", 18),
                new FootballTableRow("West Ham United FC", 19),
                new FootballTableRow("Wolverhampton Wanderers FC", 20)
            }.ToDictionary(k => k.Team.Trim(), v => v)
        ;

        private string GetJsonTemplate() =>
@"
    {{
        ""Updated"": ""{0}"",
        ""Teams"": [
{1}
        ]
    }}
";

    }
}
