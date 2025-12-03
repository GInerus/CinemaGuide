using CinemaGuide.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaGuide.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<UserMovie> UserMovies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=CinemaGuideDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальный логин
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Составной ключ в MovieGenres
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            // Внешние ключи (SQLite сам создаст в таблице)
            modelBuilder.Entity<MovieGenre>()
                .HasOne<Movie>()
                .WithMany()
                .HasForeignKey(mg => mg.MovieId);

            modelBuilder.Entity<MovieGenre>()
                .HasOne<Genre>()
                .WithMany()
                .HasForeignKey(mg => mg.GenreId);
        }
    }
}
