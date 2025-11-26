using CinemaGuide.Data;
using CinemaGuide.Helpers;
using CinemaGuide.Models;
using CinemaGuide.Views.UserControls;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CinemaGuide.ViewModels
{
    public class LoginPasswordViewModel : BaseViewModel
    {
        private CircleItem _circleItem;
        public CircleItem CircleItem
        {
            get => _circleItem;
            set
            {
                _circleItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BackgroundBrush));
                OnPropertyChanged(nameof(FirstCharacter)); // чтобы буква обновлялась
            }
        }

        public Brush BackgroundBrush => CircleItem?.Color ?? Brushes.Gray;
        public string FirstCharacter => CircleItem?.FirstCharacterOfLogin ?? "?";

        private string _selectedLogin;
        public string SelectedLogin
        {
            get => _selectedLogin;
            set
            {
                _selectedLogin = value;
                OnPropertyChanged();
                LoadCircleItemForLogin(_selectedLogin);
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string LoginText { get; set; }
        public ICommand LoginCommand { get; }
        public ICommand BackCommand { get; }

        public LoginPasswordViewModel(string login)
        {
            LoginText = login;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            BackCommand = new RelayCommand(BackToLast);

            LoadCircleItemForLogin(login);
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
                    // если пользователь не найден → показываем букву
                    CircleItem = new CircleItem { Login = login };
                }
            }
        }

        private void ExecuteLogin(object parameter)
        {
            using (var db = new AppDbContext())
            {
                if (parameter is PasswordBox passwordBox)
                {
                    string password = passwordBox.Password;
                    var user = db.Users.FirstOrDefault(u => u.Username == LoginText);
                    if (PasswordHasher.HashPassword(password) == user.PasswordHash)
                    {
                        ((MainWindow)Application.Current.MainWindow).MainContent.Content =
                            new AuthUserControl();
                    }
                    else
                    {
                        MessageBox.Show($"Пароль неверный");
                    }
                }
            }
        }


        private bool CanExecuteLogin(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                return !string.IsNullOrEmpty(passwordBox.Password);
            }
            return false;
        }


        private void BackToLast(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content =
                new AuthUserControl();
        }
    }
}
