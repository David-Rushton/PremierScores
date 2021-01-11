using System;
using System.Collections.Generic;


namespace Goals.Data.Models
{
    public class FootballMatchScore
    {
        public string Winner { get; set; }

        public FootballMatchScoreLeg FullTime { get; set; }

        public FootballMatchScoreLeg HalfTime { get; set; }

        public FootballMatchScoreLeg ExtraTime { get; set; }

        public FootballMatchScoreLeg Penalties { get; set; }

        public FootballMatchScoreLeg LiveScore =>
            new FootballMatchScoreLeg
            (
                FullTime?.HomeTeam ?? 0 + HalfTime?.HomeTeam ?? 0 + ExtraTime?.HomeTeam ?? 0 + Penalties?.HomeTeam ?? 0,
                FullTime?.AwayTeam ?? 0 + HalfTime?.AwayTeam ?? 0 + ExtraTime?.AwayTeam ?? 0 + Penalties?.AwayTeam ?? 0
            )
        ;
    }
}
