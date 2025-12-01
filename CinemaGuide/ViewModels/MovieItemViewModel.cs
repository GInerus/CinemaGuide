using CinemaGuide.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CinemaGuide.ViewModels
{
    public class MovieItemViewModel : BaseViewModel
    {
        public int MovieId { get; }
        public string Title { get; }
        public double KinopoiskRating { get; }

        private BitmapImage _posterImage;
        public BitmapImage PosterImage
        {
            get => _posterImage;
            private set
            {
                _posterImage = value;
                OnPropertyChanged(nameof(PosterImage));
            }
        }

        private string PosterFileName { get; }
        public Movie Movie { get; }

        public MovieItemViewModel(Movie movie)
        {
            Movie = movie;

            MovieId = movie.MovieId;
            Title = movie.Title;
            KinopoiskRating = movie.KinopoiskRating ?? 0;
            PosterFileName = string.IsNullOrEmpty(movie.PosterFileName) ? "no_image.png" : movie.PosterFileName;
        }

        public void LoadPoster(int PosterMaxWidth)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posters", PosterFileName);
                if (!File.Exists(path))
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posters", "no_image.png");

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = PosterMaxWidth; // уменьшаем размер постера
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();

                PosterImage = bitmap;
            }
            catch
            {
                PosterImage = null;
            }
        }

        public void UnloadPoster()
        {
            PosterImage = null;
        }
    }
}
