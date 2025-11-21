using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace CinemaGuide.Models
{
    public class CircleItem
    {
        // Предположим, что это свойство будет заполняться из БД
        public string ImagePath { get; set; }
        public String Login { get; set; }
        public bool HasValidImage => !string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath);
        public String FirstCharacterOfLogin
            => !HasValidImage ? (string.IsNullOrEmpty(Login) ? "?" : Login[0].ToString()) : "";

        public Brush Color
        {
            get
            {
                if (string.IsNullOrEmpty(Login))
                    return Brushes.Gray;

                // Если есть путь к изображению - используем картинку
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    try
                    {
                        var imageBrush = new ImageBrush();
                        imageBrush.ImageSource = new BitmapImage(new Uri(ImagePath));
                        imageBrush.Stretch = Stretch.UniformToFill;
                        imageBrush.AlignmentX = AlignmentX.Center;
                        imageBrush.AlignmentY = AlignmentY.Center;
                        return imageBrush;
                    }
                    catch
                    {
                        return GenerateColorFromLogin();
                    }
                }
                else
                {
                    return GenerateColorFromLogin();
                }    
            }
        }

        private Brush GenerateColorFromLogin()
        {
            return !string.IsNullOrEmpty(Login)
                ? (Brush)new BrushConverter().ConvertFromString(GenerateColorFromText(Login))
                : Brushes.Gray;
        }

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