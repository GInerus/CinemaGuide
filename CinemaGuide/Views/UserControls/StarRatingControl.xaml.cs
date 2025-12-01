using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CinemaGuide.Views.UserControls
{
    public partial class StarRatingControl : UserControl
    {
        public class StarFill
        {
            public Rect FillRect { get; set; }
        }

        public ObservableCollection<StarFill> Stars { get; set; } = new();

        public StarRatingControl()
        {
            InitializeComponent();
            RebuildStars();
        }

        public double Rating
        {
            get => (double)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }

        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register("Rating", typeof(double), typeof(StarRatingControl),
                new PropertyMetadata(0.0, OnRatingChanged));

        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StarRatingControl control)
                control.RebuildStars();
        }

        private void RebuildStars()
        {
            Stars.Clear();

            // переводим от 0–10 → 0–5 звёзд
            double normalized = Rating / 2.0;

            for (int i = 1; i <= 5; i++)
            {
                double fill = Math.Clamp(normalized - (i - 1), 0, 1);

                Stars.Add(new StarFill
                {
                    FillRect = new Rect(0, 0, 24 * fill, 24)
                });
            }
        }
    }
}
