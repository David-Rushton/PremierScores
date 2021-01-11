using System;
using System.Collections.Generic;


namespace Goals.Data.Models
{
    public class FootballMatchScoreLeg
    {
        public FootballMatchScoreLeg()
        { }

        public FootballMatchScoreLeg(int? homeTeam, int? awayTeam) =>
            (HomeTeam, AwayTeam) = (homeTeam ?? 0, awayTeam ?? 0)
        ;


        public int? HomeTeam { get; set; }

        public int? AwayTeam { get; set; }
    }
}
