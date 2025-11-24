using CinemaGuide.Data;
using CinemaGuide.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace CinemaGuide.ViewModels
{
    public class LoginPasswordViewModel : BaseViewModel
    {
        private string _selectedLogin;
        public string SelectedLogin
        {
            get => _selectedLogin;
            set { _selectedLogin = value; OnPropertyChanged(); }
        }

        private ImageSource _userAvatar;
        public ImageSource UserAvatar
        {
            get => _userAvatar;
            set { _userAvatar = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginPasswordViewModel(string login)
        {
            SelectedLogin = login;
            LoadAvatar(login);

            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private void LoadAvatar(string login)
        {
            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Username == login);

            string? imagePath = null;

            if (user != null && !string.IsNullOrEmpty(user.AvatarPath))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                imagePath = Path.Combine(baseDir, "avatars", user.AvatarPath);
            }

            UserAvatar = AvatarProvider.GetAvatar(imagePath, login);
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrEmpty(Password);
        }

        private void ExecuteLogin(object parameter)
        {
            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Username == SelectedLogin);

            if (user != null)
            {
                string inputHash = PasswordHasher.HashPassword(Password);
                string dbHash = user.PasswordHash;

                if (inputHash == dbHash)
                {
                    MessageBox.Show("Успешный вход!");
                }
                else
                {
                    MessageBox.Show("Неверный пароль!");
                }
            }
            else
            {
                MessageBox.Show("Пользователь не найден");
            }
        }
    }
}
