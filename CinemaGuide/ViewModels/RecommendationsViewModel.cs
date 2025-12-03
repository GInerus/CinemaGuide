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

        private const int PageSize = 50;
        private int CurrentStartIndex = 0;
        private int TotalMovies;

        public ICommand LoadNextPageCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenMovieCommand { get; }

        private User _user;
        private int _userAge;

        public RecommendationsViewModel()
        {
            LoadNextPageCommand = new AsyncCommand(LoadNextPageAsync);
            BackCommand = new RelayCommand(BackToLast);
            OpenProfileCommand = new RelayCommand(OpenProfile);
            OpenMovieCommand = new RelayCommand(OpenMovie);
        }

        public void InitializeWithUser(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userAge = CalculateUserAge(_user.BirthDate);
            _ = InitializeAsync();
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

        private async Task InitializeAsync()
        {
            using var db = new AppDbContext();
            TotalMovies = db.Movies.Count(m => m.AgeRating <= _userAge);
            await LoadNextPageAsync();
        }

        public async Task LoadNextPageAsync()
        {
            if (CurrentStartIndex >= TotalMovies)
                return;

            using var db = new AppDbContext();

            var page = db.Movies
                .Where(m => m.AgeRating <= _userAge)
                .OrderBy(m => m.MovieId)
                .Skip(CurrentStartIndex)
                .Take(PageSize)
                .ToList();

            foreach (var movieEntity in page)
            {
                var movieGenres = db.MovieGenres
                                    .Where(mg => mg.MovieId == movieEntity.MovieId)
                                    .Select(mg => db.Genres.First(g => g.GenreId == mg.GenreId))
                                    .ToList();

                var vm = new MovieItemViewModel(movieEntity)
                {
                    Genres = new ObservableCollection<Genre>(movieGenres)
                };

                Movies.Add(vm);
            }

            CurrentStartIndex += PageSize;

            await Task.Delay(5); // для плавной загрузки
        }

        private void BackToLast(object parameter)
        {
            foreach (var movie in Movies)
                movie.UnloadPoster();

            ((MainWindow)Application.Current.MainWindow).MainContent.Content =
                new AuthUserControl();
        }

        private void OpenProfile(object parameter)
        {
            foreach (var movie in Movies)
                movie.UnloadPoster();

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
    }
}
