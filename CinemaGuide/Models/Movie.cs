namespace CinemaGuide.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string ReleaseDate { get; set; } // SQLite хранит как TEXT
        public int? DurationMinutes { get; set; }
        public required int AgeRating { get; set; }
        public string? PosterFileName { get; set; }
        public double? ImdbRating { get; set; }
        public double? KinopoiskRating { get; set; }
    }
}
