namespace CinemaGuide.Models
{
    public class UserMovie
    {
        public int UserMovieId { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public UserMovieStatus Status { get; set; }
        public int? Rating { get; set; }

        public virtual User User { get; set; }
        public virtual Movie Movie { get; set; }
    }

    public enum UserMovieStatus
    {
        None = 0,
        Watching = 1,
        Planned = 2,
        Watched = 3,
        Dropped = 4
    }

}
