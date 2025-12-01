using CinemaGuide.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CinemaGuide.Helpers
{
    public static class PosterLoaderBehavior
    {
        public static readonly DependencyProperty EnableLazyLoadProperty =
            DependencyProperty.RegisterAttached(
                "EnableLazyLoad",
                typeof(bool),
                typeof(PosterLoaderBehavior),
                new PropertyMetadata(false, OnEnableLazyLoadChanged)
            );

        public static void SetEnableLazyLoad(DependencyObject element, bool value) =>
            element.SetValue(EnableLazyLoadProperty, value);

        public static bool GetEnableLazyLoad(DependencyObject element) =>
            (bool)element.GetValue(EnableLazyLoadProperty);

        private static void OnEnableLazyLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                fe.Loaded += (s, ev) =>
                {
                    if (fe.DataContext is MovieItemViewModel vm)
                        vm.LoadPoster(150);
                };

                fe.Unloaded += (s, ev) =>
                {
                    if (fe.DataContext is MovieItemViewModel vm)
                        vm.UnloadPoster();
                };
            }
        }
    }
}
