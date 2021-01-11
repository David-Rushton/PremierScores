using System;
using System.Collections.Generic;


namespace Goals.Producer.Repositories
{
    public record FootballMatches
    (
        FootballMatch[] Matches
    );


    public record FootballMatch
    (
        int Id,
        DateTime LastUpdated,
        FootballMatchTeam HomeTeam,
        FootballMatchTeam AwayTeam,
        string Status,
        int MatchDay,
        FootballMatchScore Score
    );


    public record FootballMatchTeam
    (
        int Id,
        string Name
    );


    public record FootballMatchScore
    (
        string Winner,
        FootballMatchScoreLeg FullTime,
        FootballMatchScoreLeg HalfTime,
        FootballMatchScoreLeg ExtraTime,
        FootballMatchScoreLeg Penalties
    )
    {
        public FootballMatchScoreLeg LiveScore =>
            new FootballMatchScoreLeg
            (
                FullTime?.HomeTeam ?? 0 + HalfTime?.HomeTeam ?? 0 + ExtraTime?.HomeTeam ?? 0 + Penalties?.HomeTeam ?? 0,
                FullTime?.AwayTeam ?? 0 + HalfTime?.AwayTeam ?? 0 + ExtraTime?.AwayTeam ?? 0 + Penalties?.AwayTeam ?? 0
            )
        ;
    }


    public record FootballMatchScoreLeg
    (
        int? HomeTeam,
        int? AwayTeam
    );
}
