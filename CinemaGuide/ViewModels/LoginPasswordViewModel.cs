using CinemaGuide.Data;
using CinemaGuide.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
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

                MessageBox.Show($"Введенный пароль: {Password}\n" +
                              $"Хеш введенного пароля: {inputHash}\n" +
                              $"Хеш из БД: {dbHash}\n" +
                              $"Совпадение: {inputHash == dbHash}");

                if (inputHash == dbHash)
                {
                    // Пароль верный - выполняем вход
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