using CinemaGuide.Data;
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

        private const int PageSize = 50; // Количество фильмов на одну "страницу"
        private int CurrentStartIndex = 0;
        private int TotalMovies;

        public ICommand LoadNextPageCommand { get; }

        public MoviesCatalogViewModel()
        {
            LoadNextPageCommand = new AsyncCommand(LoadNextPageAsync);
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            using var db = new AppDbContext();
            TotalMovies = db.Movies.Count();

            await LoadNextPageAsync(); // Загружаем первую страницу
        }

        public async Task LoadNextPageAsync()
        {
            if (CurrentStartIndex >= TotalMovies)
                return; // Больше страниц нет

            using var db = new AppDbContext();
            var page = db.Movies
                .OrderBy(m => m.MovieId)
                .Skip(CurrentStartIndex)
                .Take(PageSize)
                .ToList();

            foreach (var movie in page)
            {
                Movies.Add(new MovieItemViewModel(movie));
            }

            CurrentStartIndex += PageSize;

            await Task.Delay(5); // UI успевает обновиться
        }

        // Можно добавить LoadPreviousPageAsync, если хочешь подгружать вверх
    }
}
