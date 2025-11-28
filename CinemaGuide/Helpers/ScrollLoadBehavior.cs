using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CinemaGuide.Helpers
{
    public static class ScrollLoadBehavior
    {
        public static readonly DependencyProperty LoadNextPageCommandProperty =
            DependencyProperty.RegisterAttached(
                "LoadNextPageCommand",
                typeof(ICommand),
                typeof(ScrollLoadBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetLoadNextPageCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(LoadNextPageCommandProperty, value);
        }

        public static ICommand GetLoadNextPageCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(LoadNextPageCommandProperty);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScrollViewer sv) return;

            sv.ScrollChanged -= Sv_ScrollChanged; // на случай повторного присвоения
            sv.ScrollChanged += Sv_ScrollChanged;
        }

        private static void Sv_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is not ScrollViewer sv) return;

            var command = GetLoadNextPageCommand(sv);
            if (command == null) return;

            // Если почти дошли до конца
            if (sv.VerticalOffset + sv.ViewportHeight >= sv.ExtentHeight - 100)
            {
                if (command.CanExecute(null))
                    command.Execute(null);
            }
        }
    }
}
