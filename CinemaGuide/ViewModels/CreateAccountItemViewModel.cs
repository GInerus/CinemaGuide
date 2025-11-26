using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CinemaGuide.Helpers;
using CinemaGuide.Views.UserControls;

namespace CinemaGuide.ViewModels
{
    public class CreateAccountItemViewModel : BaseViewModel
    {
        public CreateAccountItem Item { get; }

        public CreateAccountItemViewModel(CreateAccountItem item)
        {
            Item = item;
            ClickCommand = new RelayCommand(OnClick);
        }

        public string Login => Item.Login;                 // "Создать"
        public string FirstCharacterOfLogin => Item.FirstCharacterOfLogin; // "+"
        public Brush Color => Item.Color;
        public Thickness TextMargin => Item.TextMargin;

        public RelayCommand ClickCommand { get; }

        private void OnClick(object obj)
        {
            // Открываем окно создания аккаунта
            ((MainWindow)Application.Current.MainWindow).MainContent.Content =
                new CreateAccountControl();   // <-- сделаешь свой UserControl
        }
    }
}
