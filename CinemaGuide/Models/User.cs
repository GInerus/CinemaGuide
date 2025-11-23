namespace CinemaGuide.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string BirthDate { get; set; } // ISO: "yyyy-MM-dd"
        public string? AvatarPath { get; set; }
    }
}
