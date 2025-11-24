using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CinemaGuide.Helpers
{
    public static class AvatarProvider
    {
        // Возвращает: либо путь к файлу, либо null
        public static ImageSource GetAvatar(string? imagePath, string username)
        {
            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                return new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            }

            // Нет картинки → возвращаем букву в виде визуального круга
            return GenerateLetterAvatar(username);
        }

        private static ImageSource GenerateLetterAvatar(string username)
        {
            string letter = string.IsNullOrWhiteSpace(username)
                ? "?"
                : username.Substring(0, 1).ToUpper();

            // Генерация изображения с буквой
            var drawing = new DrawingGroup();

            // Круг
            drawing.Children.Add(
                new GeometryDrawing(
                    new SolidColorBrush(Color.FromRgb(70, 70, 70)),
                    null,
                    new EllipseGeometry(new System.Windows.Rect(0, 0, 256, 256)))
            );

            // Буква
            var formatted = new FormattedText(
                letter,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                140,
                Brushes.White,
                1.0);

            drawing.Children.Add(
                new GeometryDrawing(
                    Brushes.White,
                    null,
                    formatted.BuildGeometry(new System.Windows.Point(70, 40)))
            );

            var bmp = new DrawingImage(drawing);
            bmp.Freeze();
            return bmp;
        }
    }
}
