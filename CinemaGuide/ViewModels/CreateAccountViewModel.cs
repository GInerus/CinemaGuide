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
    public class CreateAccountViewModel : BaseViewModel
    {
        private CircleItem _circleItem;

        private string _selectedLogin;
        public string SelectedLogin
        {
            get => _selectedLogin;
            set
            {
                _selectedLogin = value;
                OnPropertyChanged();
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

        public CreateAccountViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            BackCommand = new RelayCommand(BackToLast);
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
