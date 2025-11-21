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

        public RelayCommand GoToAuthCommand { get; }

        public MainWindowViewModel()
        {
            GoToAuthCommand = new RelayCommand(o =>
            {
                CurrentView = new AuthPageViewModel();
            });

            CurrentView = new AuthPageViewModel(); // при старте
        }
    }

}
