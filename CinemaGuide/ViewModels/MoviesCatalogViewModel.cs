using CinemaGuide.Data;
using CinemaGuide.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CinemaGuide.Helpers;

namespace CinemaGuide.ViewModels
{
    public class MoviesCatalogViewModel : BaseViewModel
    {
        public ObservableCollection<MovieItemViewModel> Movies { get; set; } = new();

        private const int PageSize = 50;
        private int CurrentStartIndex = 0;
        private int TotalMovies;

        public ICommand LoadNextPageCommand { get; }

        private User _user;
        private int _userAge;

        public MoviesCatalogViewModel()
        {
            LoadNextPageCommand = new AsyncCommand(LoadNextPageAsync);
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
    }
}
