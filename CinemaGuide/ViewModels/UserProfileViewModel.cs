using CinemaGuide.Models;
using System.Windows.Input;
using System.Windows;
using CinemaGuide.Views.UserControls;
using CinemaGuide.Data;
using CinemaGuide.Helpers;

namespace CinemaGuide.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        public User User { get; }

        public ICommand BackCommand { get; }

        public UserProfileViewModel(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));

            BackCommand = new RelayCommand(BackToPrevious);
        }

        private void BackToPrevious(object parameter)
        {
            // Создаём UserControl для каталога фильмов
            var catalogControl = new MoviesCatalogControl();
            // Получаем ViewModel и передаём пользователя
            if (catalogControl.DataContext is MoviesCatalogViewModel catalogVM)
            {
                catalogVM.InitializeWithUser(User);
            }
                        // Вставляем UserControl в MainContent
                        ((MainWindow)Application.Current.MainWindow).MainContent.Content = catalogControl;
        }
    }
}
