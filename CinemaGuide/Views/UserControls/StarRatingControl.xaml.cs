using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void Star_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this); // позиция относительно всего контролла
            double starWidth = ActualWidth / 5.0;
            double value = pos.X / starWidth;
            value = Math.Clamp(value, 0, 5);
            value = Math.Round(value); // округляем до целого
            Rating = value * 2; // 0–10
        }


        // Логика клика мышью для интерактивных звезд
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                UpdateRatingFromMouse(e.GetPosition(this).X);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            UpdateRatingFromMouse(e.GetPosition(this).X);
        }

        private void UpdateRatingFromMouse(double mouseX)
        {
            double starWidth = ActualWidth / 5.0;
            double value = mouseX / starWidth;

            // Ограничиваем 0..5
            value = Math.Clamp(value, 0, 5);

            // Округляем до половинок
            value = Math.Round(value * 2) / 2;

            // Если кликнули точно в конец, ставим максимум
            if (value > 4.9) value = 5.0;

            Rating = value * 2; // обратно в 0..10
        }


    }
}
