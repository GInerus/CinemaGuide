using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CinemaGuide
{
    public class CircleItem
    {
        public Brush Color
            => !string.IsNullOrEmpty(Login) ? (Brush)new BrushConverter().ConvertFromString(GenerateColorFromText(Login)) : Brushes.Gray;
        public String Login { get; set; }
        public String FirstCharacterOfLogin 
            => !string.IsNullOrEmpty(Login) ? Login[0].ToString() : "";

        public static string GenerateColorFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "#808080";

            // Яркие насыщенные цвета из палитры
            string[] colorPalette = {
        "#FF0000", // Красный
        "#FF8000", // Оранжевый
        "#FFFF00", // Желтый
        "#80FF00", // Ярко-зеленый
        "#00FF00", // Зеленый
        "#00FF80", // Бирюзовый
        "#00FFFF", // Голубой
        "#0080FF", // Синий
        "#0000FF", // Синий (насыщенный)
        "#8000FF", // Фиолетовый
        "#FF00FF", // Пурпурный
        "#FF0080"  // Розовый
    };

            int hash = Math.Abs(text.GetHashCode());
            int index = hash % colorPalette.Length;

            return colorPalette[index];
        }
    }
}