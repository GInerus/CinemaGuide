using CinemaGuide.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using CinemaGuide.Data;

namespace CinemaGuide.ViewModels
{
    public class MoviePageViewModel : BaseViewModel
    {
        public MovieItemViewModel MovieItem { get; }

        public int MovieId => MovieItem.MovieId;
        public string Title => MovieItem.Title;
        public BitmapImage Poster => MovieItem.PosterImage;
        public double KinopoiskRating => MovieItem.KinopoiskRating;
        public string Description => MovieItem.Movie.Description;
        public string ReleaseYear { get; private set; }
        public string DurationFormatted { get; private set; }
        public int AgeRating { get; private set; }
        public double ImdbRating => MovieItem.Movie.ImdbRating ?? 0;
        public ObservableCollection<string> Genres { get; private set; } = new();

        public MoviePageViewModel(MovieItemViewModel movieItem)
        {
            MovieItem = movieItem ?? throw new ArgumentNullException(nameof(movieItem));

            // Дата релиза
            if (DateTime.TryParse(MovieItem.Movie.ReleaseDate, out var date))
                ReleaseYear = date.Year.ToString();
            else
                ReleaseYear = "—";

            // Длительность
            var dur = MovieItem.Movie.DurationMinutes ?? 0;
            DurationFormatted = $"{dur / 60} ч {dur % 60} мин";

            AgeRating = MovieItem.Movie.AgeRating;

            LoadGenres(MovieItem.Movie.MovieId);
        }

        private void LoadGenres(int movieId)
        {
            try
            {
                using var db = new AppDbContext();
                var genres = db.MovieGenres
                    .Where(mg => mg.MovieId == movieId)
                    .Join(db.Genres, mg => mg.GenreId, g => g.GenreId,
                          (mg, g) => g.Name)
                    .ToList();

                Genres.Clear();
                foreach (var g in genres)
                    Genres.Add(g);
            }
            catch
            {
                Genres.Clear();
            }
        }
    }
}