using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace CinemaGuide.Converters
{
    public class ActiveTabToBrushConverter : IValueConverter
    {
        public Brush ActiveBrush { get; set; } = Brushes.White;
        public Brush InactiveBrush { get; set; } = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value?.ToString();
            var target = parameter?.ToString();

            return string.Equals(current, target, StringComparison.OrdinalIgnoreCase)
                ? ActiveBrush
                : InactiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
