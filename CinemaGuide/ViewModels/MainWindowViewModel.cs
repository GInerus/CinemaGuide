using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaGuide.Helpers;

namespace CinemaGuide.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private BaseViewModel currentView;
        public BaseViewModel CurrentView
        {
            get => currentView;
            set { currentView = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            CurrentView = new AuthPageViewModel(); // стартовый экран
        }

        public void GoToLoginPassword(string login)
        {
            CurrentView = new LoginPasswordViewModel(login);
        }
    }

}