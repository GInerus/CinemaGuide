using CinemaGuide.Data;
using CinemaGuide.Helpers;
using CinemaGuide.Models;
using CinemaGuide.Views.UserControls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CinemaGuide.ViewModels;

namespace CinemaGuide.ViewModels
{
    public class MoviesCatalogViewModel : BaseViewModel
    {
        public ObservableCollection<MovieItemViewModel> Movies { get; set; } = new();

        private const int PageSize = 50;
        private int CurrentStartIndex = 0;
        private int TotalMovies;

        public ICommand LoadNextPageCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand OpenProfileCommand {get; }
        public ICommand OpenMovieCommand { get; }
        public ICommand SwitchTabCommand { get; }  // команда для переключения вкладок

        private User _user;
        private int _userAge;

        private string _currentTab;
        public string CurrentTab
        {
            get => _currentTab;
            set
            {
                if (_currentTab != value)
                {
                    _currentTab = value;
                    OnPropertyChanged(nameof(CurrentTab));
                }
            }
        }
        private bool _isSearchVisible;
        public bool IsSearchVisible
        {
            get => _isSearchVisible;
            set
            {
                if (_isSearchVisible != value)
                {
                    _isSearchVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFiltersVisible;
        public bool IsFiltersVisible
        {
            get => _isFiltersVisible;
            set
            {
                if (_isFiltersVisible != value)
                {
                    _isFiltersVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        // Команды для кнопок поиска и фильтров
        public ICommand ToggleSearchCommand => new RelayCommand(_ =>
        {
            IsSearchVisible = !IsSearchVisible;
            if (IsSearchVisible)
                IsFiltersVisible = false; // фильтры прячем, если показан поиск
        });

        public ICommand ToggleFiltersCommand => new RelayCommand(_ =>
        {
            IsFiltersVisible = !IsFiltersVisible;
            if (IsFiltersVisible)
                IsSearchVisible = false; // поиск прячем, если показаны фильтры
        });

        public MoviesCatalogViewModel()
        {
            LoadNextPageCommand = new AsyncCommand(LoadNextPageAsync);
            BackCommand = new RelayCommand(BackToLast);
            OpenProfileCommand = new RelayCommand(OpenProfile);
            OpenMovieCommand = new RelayCommand(OpenMovie);


            // Команда переключения вкладок
            SwitchTabCommand = new RelayCommand(tab =>
            {
                if (tab is string tabName)
                    CurrentTab = tabName;
            });

            // Устанавливаем вкладку "Каталог" активной по умолчанию
            CurrentTab = "Catalog";
        }

        public void InitializeWithUser(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userAge = CalculateUserAge(_user.BirthDate);
            InitializeAsync();
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

            foreach (var movie in page)
                Movies.Add(new MovieItemViewModel(movie));

            CurrentStartIndex += PageSize;

            await Task.Delay(5);
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
