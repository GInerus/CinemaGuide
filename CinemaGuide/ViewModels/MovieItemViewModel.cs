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

            // Сначала пусто, картинка загрузится асинхронно
            PosterPath = "";

            LoadPosterAsync(movie.PosterFileName);
        }

        private async void LoadPosterAsync(string fileName)
        {
            await Task.Delay(10); // даём UI время нарисовать карточку

            if (!string.IsNullOrEmpty(fileName))
            {
                string path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Posters",
                    fileName
                );

                if (File.Exists(path))
                    PosterPath = path;
                else
                    PosterPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Posters",
                        "no_image.png" // заглушка при отсутствии файла
                    );
            }
            else
            {
                PosterPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Posters",
                    "no_image.png"
                );
            }
        }
    }
}
