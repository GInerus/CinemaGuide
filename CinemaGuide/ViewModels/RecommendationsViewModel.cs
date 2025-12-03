using CinemaGuide.Data;
using CinemaGuide.Helpers;
using CinemaGuide.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CinemaGuide.ViewModels
{
    public class RecommendationsViewModel : BaseViewModel
    {
        public ObservableCollection<MovieItemViewModel> Movies { get; set; } = new();

        public ICommand LoadNextPageCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand SwitchTabCommand { get; }

        private User _user;
        private int _userAge;

        public RecommendationsViewModel()
        {
            LoadNextPageCommand = new AsyncCommand(LoadNextPageAsync);
            BackCommand = new RelayCommand(BackToLast);
            OpenProfileCommand = new RelayCommand(OpenProfile);
            SwitchTabCommand = new RelayCommand(tab =>
            {
                if (tab is string tabName)
                    CurrentTab = tabName;
            });
        }

        public void InitializeWithUser(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userAge = CalculateUserAge(_user.BirthDate);
            _ = LoadNextPageAsync();
        }

        private int CalculateUserAge(string birthDate)
        {
            if (DateTime.TryParse(birthDate, out var dob))
            {
                int age = DateTime.Today.Year - dob.Year;
                if (dob.Date > DateTime.Today.AddYears(-age)) age--;
                return age;
            }
            return 0;
        }

        private async Task LoadNextPageAsync()
        {
            using var db = new AppDbContext();
            var movies = db.Movies
                           .Where(m => m.AgeRating <= _userAge)
                           .OrderByDescending(m => m.KinopoiskRating)
                           .Take(10)
                           .ToList();

            Movies.Clear();
            foreach (var m in movies)
            {
                var genres = db.MovieGenres
                               .Where(mg => mg.MovieId == m.MovieId)
                               .Select(mg => db.Genres.First(g => g.GenreId == mg.GenreId))
                               .ToList();

                Movies.Add(new MovieItemViewModel(m)
                {
                    Genres = new ObservableCollection<Genre>(genres)
                });
            }

            await Task.Delay(5);
        }

        private void BackToLast(object parameter) { /* То же, что в каталоге */ }
        private void OpenProfile(object parameter) { /* То же, что в каталоге */ }

        private string _currentTab;
        public string CurrentTab
        {
            get => _currentTab;
            set { _currentTab = value; OnPropertyChanged(); }
        }
    }

}
