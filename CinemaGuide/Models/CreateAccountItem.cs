using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CinemaGuide.Models
{
    public class CreateAccountItem
    {
        public Brush Color => Brushes.White;  // Стандартный серый цвет
        public string Login => "Создать";
        public string FirstCharacterOfLogin => "+";  // Плюсик вместо буквы
        public Thickness TextMargin => new Thickness(0, 0, 0, 10); // Смещение для плюса
    }
}
