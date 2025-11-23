using CinemaGuide.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace CinemaGuide.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=CinemaGuideDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique(); // Уникальный логин
        }
    }
}
