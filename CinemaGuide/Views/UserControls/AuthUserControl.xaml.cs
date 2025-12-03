using CinemaGuide.ViewModels;
using System.Windows.Controls;

namespace CinemaGuide.Views.UserControls
{
    public partial class AuthUserControl : UserControl
    {
        public AuthUserControl()
        {
            InitializeComponent();
            DataContext = new AuthPageViewModel();
        }
    }
}
