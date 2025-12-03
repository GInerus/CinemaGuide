using CinemaGuide.Data;
using CinemaGuide.Helpers;
using CinemaGuide.Models;
using CinemaGuide.Views.UserControls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CinemaGuide.ViewModels
{
    public class RecommendationsViewModel : BaseViewModel
    {
        public ObservableCollection<MovieItemViewModel> Movies { get; set; } = new();

        private int TotalMovies;

        public ICommand BackCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenMovieCommand { get; }
        public ICommand SwitchTabCommand { get; }  // Для возврата в каталог


        private User _user;
        private int _userAge;

        public RecommendationsViewModel()
        {
            BackCommand = new RelayCommand(BackToLast);
            OpenProfileCommand = new RelayCommand(OpenProfile);
            OpenMovieCommand = new RelayCommand(OpenMovie);
            // Команда для возврата в каталог
            SwitchTabCommand = new RelayCommand(_ => OpenCatalog());
        }

        public void InitializeWithUser(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userAge = CalculateUserAge(_user.BirthDate);
            LoadRecommendations(); // сразу загружаем рекомендации
        }

        private int CalculateUserAge(string birthDate)
        {
            if (DateTime.TryParse(birthDate, out var dob))
            {
                var today = DateTime.Today;
                int age = today.Year - dob.Year;
                if (dob.Date > today.AddYears(-age)) age--;
                return age;
            }
            return 0;
        }

        private void LoadRecommendations()
        {
            Movies.Clear();

            using var db = new AppDbContext();

            // 1. Топ-5 просмотренных и оцененных фильмов
            var watchedRated = db.UserMovies
                .Where(um => um.UserId == _user.UserId && um.Status == UserMovieStatus.Watched && um.Rating.HasValue)
                .OrderByDescending(um => um.Rating)
                .Take(5)
                .ToList();

            if (!watchedRated.Any())
                return; // если нет оцененных фильмов, ничего не показываем

            // 2. Собираем жанры
            var preferredGenreIds = watchedRated
                .SelectMany(um => db.MovieGenres
                    .Where(mg => mg.MovieId == um.MovieId)
                    .Select(mg => mg.GenreId))
                .Distinct()
                .ToList();

            var alreadyWatchedIds = watchedRated.Select(um => um.MovieId).ToList();

            // 3. Находим фильмы с этими жанрами и сортируем по рейтингу
            var recommendedMovies = db.Movies
                .Where(m => m.AgeRating <= _userAge &&
                            !alreadyWatchedIds.Contains(m.MovieId) &&
                            db.MovieGenres.Any(mg => mg.MovieId == m.MovieId && preferredGenreIds.Contains(mg.GenreId)))
                .OrderByDescending(m => m.KinopoiskRating)
                .Take(15)
                .ToList();

            foreach (var movie in recommendedMovies)
            {
                var movieGenres = db.MovieGenres
                                    .Where(mg => mg.MovieId == movie.MovieId)
                                    .Select(mg => db.Genres.First(g => g.GenreId == mg.GenreId))
                                    .ToList();

                var vm = new MovieItemViewModel(movie)
                {
                    Genres = new ObservableCollection<Genre>(movieGenres)
                };
                Movies.Add(vm);
            }
        }

        private void BackToLast(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content =
                new AuthUserControl();
        }

        private void OpenProfile(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content =
                new UserProfileControl(_user);
        }

        public void OpenMovie(object parameter)
        {
            if (parameter is not int movieId)
            {
                MessageBox.Show("Ожидался MovieId (int)");
                return;
            }

            using var db = new AppDbContext();
            var movie = db.Movies.FirstOrDefault(m => m.MovieId == movieId);
            if (movie == null)
            {
                MessageBox.Show("Фильм не найден");
                return;
            }

            var moviePage = new MoviePageControl();
            moviePage.DataContext = new MoviePageViewModel(new MovieItemViewModel(movie), _user);

            ((MainWindow)Application.Current.MainWindow).MainContent.Content = moviePage;
        }

        private void OpenCatalog()
        {
            // Создаём UserControl каталога
            var catalogControl = new MoviesCatalogControl();

            // Инициализируем ViewModel с текущим пользователем
            var vm = new MoviesCatalogViewModel();
            vm.InitializeWithUser(_user);
            catalogControl.DataContext = vm;

            // Меняем содержимое MainContent
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = catalogControl;
        }
    }
}
