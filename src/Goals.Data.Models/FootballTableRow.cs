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
            Points = 0;
        }


        public string Team { get; set; }

        public int Position { get; set; }

        public int Wins { get; set; }

        public int Loses { get; set; }

        public int Draws { get; set; }

        public int Points { get; set; }
    }
}
