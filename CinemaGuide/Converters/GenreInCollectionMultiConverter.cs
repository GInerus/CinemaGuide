using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;
using CinemaGuide.Models;

namespace CinemaGuide.Converters
{
    public class GenreInCollectionMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedGenres = values[0] as ObservableCollection<Genre>;
            var genre = values[1] as Genre;
            if (selectedGenres == null || genre == null)
                return false;
            return selectedGenres.Contains(genre);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var isChecked = (bool)value;
            // targetTypes[0] — SelectedGenres
            // targetTypes[1] — Genre
            return null; // мы обработаем событием в VM
        }
    }
}
