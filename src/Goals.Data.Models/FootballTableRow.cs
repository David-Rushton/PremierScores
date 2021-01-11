namespace Goals.Data.Models
{
    public class FootballTableRow
    {
        public FootballTableRow(string team, int position)
        {
            Team = team;
            Position = position;
            Wins = 0;
            Loses = 0;
            Draws = 0;
        }


        public string Team { get; set; }

        public int Position { get; set; }

        public int Wins { get; set; }

        public int Loses { get; set; }

        public int Draws { get; set; }

        public int Points => ((Wins * 3) + Draws);


        public string ToJson() =>
            string.Format
            (
                "{{ \"position\": {0}, \"team\": \"{1}\", \"won\": {2}, \"drawn\": {3}, \"lost\": {4}, \"points\": {5} }}",
                Position.ToString().PadLeft(2),
                Team.PadRight(27),
                Wins.ToString().PadLeft(2),
                Draws.ToString().PadLeft(2),
                Loses.ToString().PadLeft(2),
                Points.ToString().PadLeft(2)
            )
        ;
    }
}
