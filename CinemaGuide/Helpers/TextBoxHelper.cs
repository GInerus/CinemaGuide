using System.Windows;
using System.Windows.Controls;

namespace CinemaGuide.Helpers
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached(
                "Watermark",
                typeof(string),
                typeof(TextBoxHelper),
                new PropertyMetadata(default(string)));

        public static void SetWatermark(UIElement element, string value)
        {
            element.SetValue(WatermarkProperty, value);
        }

        public static string GetWatermark(UIElement element)
        {
            return (string)element.GetValue(WatermarkProperty);
        }
    }
}
