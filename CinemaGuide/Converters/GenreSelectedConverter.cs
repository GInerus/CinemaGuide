using CinemaGuide.Models;
using CinemaGuide.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace CinemaGuide.Converters
{
    public class GenreSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value — SelectedGenres
            var selectedGenres = value as ObservableCollection<Genre>;
            var genre = parameter as Genre; // берем через Tag или DataContext

            if (selectedGenres == null || genre == null)
                return false;

            return selectedGenres.Contains(genre);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isChecked = (bool)value;
            var genre = parameter as Genre;
            var selectedGenres = App.Current.MainWindow.DataContext as MoviesCatalogViewModel; // или передаем через MultiBinding

            if (genre == null || selectedGenres == null)
                return null;

            // Добавляем/удаляем жанр из коллекции
            if (isChecked)
            {
                if (!selectedGenres.SelectedGenres.Contains(genre))
                    selectedGenres.SelectedGenres.Add(genre);
            }
            else
            {
                if (selectedGenres.SelectedGenres.Contains(genre))
                    selectedGenres.SelectedGenres.Remove(genre);
            }

            return null; // не возвращаем новую коллекцию
        }
    }
}
