using CinemaGuide.Data;
using CinemaGuide.Helpers;
using CinemaGuide.Models;
using CinemaGuide.Views.UserControls;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;

namespace CinemaGuide.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        public User User { get; }
        public CircleItem CircleItem { get; private set; }


        public ICommand BackCommand { get; }

        public UserProfileViewModel(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));

            BackCommand = new RelayCommand(BackToPrevious);

            LoadCircleItemForLogin(user.Username);
        }

        private void LoadCircleItemForLogin(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                CircleItem = new CircleItem { Login = "?" };
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == login);

                if (user != null)
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string imagePath = Path.Combine(baseDir, "avatars", user.AvatarPath ?? "");

                    CircleItem = new CircleItem
                    {
                        Login = user.Username,
                        ImagePath = File.Exists(imagePath) ? imagePath : null
                    };
                }
                else
                {
                    CircleItem = new CircleItem { Login = login };
                }
            }

            OnPropertyChanged(nameof(CircleItem));
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
