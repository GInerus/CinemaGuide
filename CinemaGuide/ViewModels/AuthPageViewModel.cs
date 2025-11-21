using System.Collections.ObjectModel;
using CinemaGuide.Models;

namespace CinemaGuide.ViewModels
{
    public class AuthPageViewModel : BaseViewModel
    {
        public ObservableCollection<object> Circles { get; set; }

        public AuthPageViewModel()
        {
            Circles = new ObservableCollection<object>
            {
                new CircleItemViewModel(new CircleItem { Login = "Дмитрий", ImagePath = @"C:\Users\germa\OneDrive\Изображения\Ава\Dima.jpg" }),
                new CircleItemViewModel(new CircleItem { Login = "Герман", ImagePath = @"C:\Users\germa\OneDrive\Изображения\Ава\Xeno-.jpg" }),
                new CircleItemViewModel(new CircleItem { Login = "Евгений", ImagePath = @"C:\Users\germa\OneDrive\Изображения\Ава\Jeka.jpg" }),
                new CircleItemViewModel(new CircleItem { Login = "Данила", ImagePath = @"C:\Users\germa\OneDrive\Изображения\Ава\Dany.jpg" }),
                new CircleItemViewModel(new CircleItem { Login = "Ух" }),
                
                // Добавляем "плюсик" для создания аккаунта
                new CreateAccountItem()
            };
        }
    }
}
