using CinemaGuide.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CinemaGuide.ViewModels
{
    public class MovieItemViewModel : BaseViewModel
    {
        public int MovieId { get; }
        public string Title { get; }
        public double KinopoiskRating { get; }

        private string _posterPath;
        public string PosterPath
        {
            get => _posterPath;
            private set
            {
                _posterPath = value;
                OnPropertyChanged(nameof(PosterPath));
            }
        }

        public MovieItemViewModel(Movie movie)
        {
            MovieId = movie.MovieId;
            Title = movie.Title;
            KinopoiskRating = movie.KinopoiskRating ?? 0; // если null, ставим 0

            PosterPath = "";
            LoadPosterAsync(movie.PosterFileName);
        }

        private async void LoadPosterAsync(string fileName)
        {
            await Task.Delay(10);

            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Posters",
                string.IsNullOrEmpty(fileName) ? "no_image.png" : fileName
            );

            if (!File.Exists(path))
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posters", "no_image.png");

            PosterPath = path;
        }
    }
}
