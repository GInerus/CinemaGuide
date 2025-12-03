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
        public ObservableCollection<Genre> Genres { get; set; } = new();

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

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplySearch(); // обновляем список фильмов при вводе
                }
            }
        }

        // Хранит все фильмы без фильтра
        private ObservableCollection<MovieItemViewModel> _allMovies = new();

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // если поиск пустой — показываем все
                Movies.Clear();
                foreach (var m in _allMovies)
                    Movies.Add(m);
            }
            else
            {
                var filtered = _allMovies
                    .Where(m => m.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                Movies.Clear();
                foreach (var m in filtered)
                    Movies.Add(m);
            }
        }

        // Список всех жанров для панели фильтров
        public ObservableCollection<Genre> AllGenres { get; set; } = new();

        // Выбранные жанры
        private ObservableCollection<Genre> _selectedGenres = new();
        public ObservableCollection<Genre> SelectedGenres
        {
            get => _selectedGenres;
            set
            {
                if (_selectedGenres != value)
                {
                    if (value != null)
                    {
                        _selectedGenres.Clear();
                        foreach (var g in value)
                            _selectedGenres.Add(g);
                    }

                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }
        public ICommand ToggleGenreCommand => new RelayCommand(param =>
        {
            if (param is Genre genre)
            {
                if (SelectedGenres.Contains(genre))
                    SelectedGenres.Remove(genre);
                else
                    SelectedGenres.Add(genre);

                ApplyFilters();
            }
        });

        // Метод фильтрации по жанрам
        private void ApplyFilters()
        {
            IEnumerable<MovieItemViewModel> filtered = _allMovies;

            if (SelectedGenres.Any())
            {
                var selectedIds = SelectedGenres.Select(g => g.GenreId).ToList();
                filtered = filtered.Where(m =>
                    m.Genres.Any(g => selectedIds.Contains(g.GenreId))
                );
            }

            Movies.Clear();
            foreach (var m in filtered)
                Movies.Add(m);
        }


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

            AllGenres.Clear();
            foreach (var genre in db.Genres.ToList())
                AllGenres.Add(genre);
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

            foreach (var movieEntity in page) // переименовали переменную
            {
                // Получаем жанры для фильма
                var movieGenres = db.MovieGenres
                                    .Where(mg => mg.MovieId == movieEntity.MovieId)
                                    .Select(mg => db.Genres.First(g => g.GenreId == mg.GenreId))
                                    .ToList();

                var vm = new MovieItemViewModel(movieEntity)
                {
                    Genres = new ObservableCollection<Genre>(movieGenres)
                };

                Movies.Add(vm);
                _allMovies.Add(vm); // вспомогательная коллекция
            }

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
