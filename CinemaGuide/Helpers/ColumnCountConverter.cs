using System;
using System.Globalization;
using System.Windows.Data;

namespace CinemaGuide.Helpers
{
    public class ColumnCountConverter : IValueConverter
    {
        public double MinItemWidth { get; set; } = 160; // минимальная ширина карточки (вместе с Margin)

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double actualWidth && actualWidth > 0)
            {
                int columns = (int)(actualWidth / MinItemWidth);
                return Math.Max(columns, 1); // минимум 1 столбец
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
