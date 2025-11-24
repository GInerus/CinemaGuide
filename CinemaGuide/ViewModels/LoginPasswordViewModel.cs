using CinemaGuide.Data;
using CinemaGuide.Helpers;
using System.Windows;
using System.Windows.Input;
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

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public LoginPasswordViewModel(string login)
        {
            // Тут загрузка даных
            MessageBox.Show("Нажата");


            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }


        public ICommand LoginCommand { get; }

        private void ExecuteLogin(object parameter) 
        {
            MessageBox.Show("Нажата кнопка");
        }
        private bool CanExecuteLogin(object parameter)
        {
            return true;
            //return !string.IsNullOrEmpty(Password);
        }
    }
}
