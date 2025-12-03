using CinemaGuide.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CinemaGuide.Data;
using CinemaGuide.ViewModels;
using CinemaGuide.Views.UserControls;
using CinemaGuide.Helpers; // RelayCommand

namespace CinemaGuide.ViewModels
{
    public class MoviePageViewModel : BaseViewModel
    {
        public MovieItemViewModel MovieItem { get; }
        public User User { get; }
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
        public UserMovieStatus CurrentStatus { get; set; }
        public int? CurrentRating { get; set; }
        private double _userRating;
        public double UserRating
        {
            get => _userRating;
            set
            {
                if (_userRating != value)
                {
                    _userRating = value;
                    OnPropertyChanged(); // уведомляем интерфейс
                    SaveUserRating();
                }
            }
        }

        public ICommand SetStatusCommand { get; }
        public ICommand SetRatingCommand { get; }

        public ICommand BackCommand { get; }

        public MoviePageViewModel(MovieItemViewModel movieItem, User user)
        {
            MovieItem = movieItem ?? throw new ArgumentNullException(nameof(movieItem));
            User = user ?? throw new ArgumentNullException(nameof(user));

            // Команда возврата к каталогу фильмов
            BackCommand = new RelayCommand(BackToCatalog);
            // Команда для установки статуса просмотра
            SetStatusCommand = new RelayCommand(SetStatus);
            // Команда для установки рейтинга фильма от пользователя
            SetRatingCommand = new RelayCommand(SetRating);

            if (MovieItem.PosterImage == null)
                MovieItem.LoadPoster(200);

            if (DateTime.TryParse(MovieItem.Movie.ReleaseDate, out var date))
                ReleaseYear = date.Year.ToString();
            else
                ReleaseYear = "—";

            var dur = MovieItem.Movie.DurationMinutes ?? 0;
            DurationFormatted = $"{dur / 60} ч {dur % 60} мин";

            AgeRating = MovieItem.Movie.AgeRating;

            LoadGenres(MovieItem.Movie.MovieId);
            LoadUserMovieData();
        }

        private void LoadUserMovieData()
        {
            using var db = new AppDbContext();
            var userMovie = db.UserMovies.FirstOrDefault(um => um.UserId == User.UserId && um.MovieId == MovieId);
            if (userMovie != null)
            {
                CurrentStatus = userMovie.Status;
                CurrentRating = userMovie.Rating;
                UserRating = userMovie.Rating.GetValueOrDefault();
            }
        }

        private void SetStatus(object statusObj)
        {
            if (statusObj is UserMovieStatus status)
            {
                CurrentStatus = status;

                using var db = new AppDbContext();
                var userMovie = db.UserMovies.FirstOrDefault(um => um.UserId == User.UserId && um.MovieId == MovieId);
                if (userMovie == null)
                {
                    userMovie = new UserMovie
                    {
                        UserId = User.UserId,
                        MovieId = MovieId,
                        Status = status
                    };
                    db.UserMovies.Add(userMovie);
                }
                else
                {
                    userMovie.Status = status;
                }
                db.SaveChanges();
            }
        }

        private void SetRating(object parameter)
        {
            if (parameter is int rating)
            {
                CurrentRating = rating;

                using var db = new AppDbContext();
                var userMovie = db.UserMovies.FirstOrDefault(um => um.UserId == User.UserId && um.MovieId == MovieId);

                if (userMovie == null)
                {
                    userMovie = new UserMovie
                    {
                        UserId = User.UserId,
                        MovieId = MovieId,
                        Rating = rating
                    };
                    db.UserMovies.Add(userMovie);
                }
                else
                {
                    userMovie.Rating = rating;
                }
                db.SaveChanges();
            }
        }

        private void SaveUserRating()
        {
            using var db = new AppDbContext();
            var userMovie = db.UserMovies.FirstOrDefault(um => um.UserId == User.UserId && um.MovieId == MovieId);

            if (userMovie == null)
            {
                userMovie = new UserMovie
                {
                    UserId = User.UserId,
                    MovieId = MovieId,
                    Rating = (int)Math.Round(UserRating)
                };
                db.UserMovies.Add(userMovie);
            }
            else
            {
                userMovie.Rating = (int)Math.Round(UserRating);
            }
            db.SaveChanges();
        }


        private void BackToCatalog(object parameter)
        {
            try
            {
                // Создаём UserControl каталога
                var catalogControl = new Views.UserControls.MoviesCatalogControl();

                // Передаём ViewModel, если нужно
                if (catalogControl.DataContext is ViewModels.MoviesCatalogViewModel catalogVM)
                {
                    catalogVM.InitializeWithUser(User);
                }

                // Меняем содержимое MainContent
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainContent.Content = catalogControl;
                }
            }
            catch
            {
                MessageBox.Show("Не удалось вернуться к каталогу фильмов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
