using CinemaGuide.Data;
using CinemaGuide.Helpers;
using System.Windows;
using System.Windows.Input;
using System.IO;
using CinemaGuide.Views.UserControls;

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

        public LoginPasswordViewModel(string login)
        {
            // Тут загрузка даных
            LoginText = login; 
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            BackCommand = new RelayCommand(BackToLast);
        }

        public string LoginText { get; set; }
        public ICommand LoginCommand { get; }
        public ICommand BackCommand { get; }

        private void ExecuteLogin(object parameter) 
        {
            MessageBox.Show("Нажата кнопка");
        }
        private bool CanExecuteLogin(object parameter)
        {
            return true;
            //return !string.IsNullOrEmpty(Password);
        }
        private void BackToLast(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content =
                new AuthUserControl();
        }
    }
}
