using System;
using System.Collections.Generic;


namespace Goals.Data.Models
{
    public class FootballMatch
    {
        public int Id { get; set; }

        public DateTime LastUpdated { get; set; }

        public FootballMatchTeam HomeTeam { get; set; }

        public FootballMatchTeam AwayTeam { get; set; }

        public string Status { get; set; }

        public int MatchDay { get; set; }

        public FootballMatchScore Score { get; set; }

    }
}
