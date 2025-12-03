using System.Windows;
using System.Windows.Controls;

namespace CinemaGuide.Helpers
{
    public static class PasswordBoxHelper
    {
        // Существующие свойства для привязки пароля
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPasswordProperty =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxHelper));

        // НОВОЕ: Свойство для watermark текста
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(PasswordBoxHelper),
                new PropertyMetadata(null));

        // НОВОЕ: Свойство для отслеживания watermark видимости
        public static readonly DependencyProperty ShowWatermarkProperty =
            DependencyProperty.RegisterAttached("ShowWatermark", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(true));

        public static void SetWatermark(DependencyObject dp, string value)
        {
            dp.SetValue(WatermarkProperty, value);
        }

        public static string GetWatermark(DependencyObject dp)
        {
            return (string)dp.GetValue(WatermarkProperty);
        }

        public static void SetShowWatermark(DependencyObject dp, bool value)
        {
            dp.SetValue(ShowWatermarkProperty, value);
        }

        public static bool GetShowWatermark(DependencyObject dp)
        {
            return (bool)dp.GetValue(ShowWatermarkProperty);
        }

        // Существующие методы остаются без изменений...
        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPasswordProperty, value);
        }

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPasswordProperty);
        }

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPasswordProperty, value);
        }

        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPasswordProperty);
        }

        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = d as PasswordBox;

            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            box.PasswordChanged -= HandlePasswordChanged;

            string newPassword = (string)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword;
            }

            // НОВОЕ: Обновляем видимость watermark
            UpdateWatermarkVisibility(box);

            box.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            bool wasBound = (bool)e.OldValue;
            bool needToBind = (bool)e.NewValue;

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
                box.GotFocus -= OnPasswordBoxGotFocus;
                box.LostFocus -= OnPasswordBoxLostFocus;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
                box.GotFocus += OnPasswordBoxGotFocus;
                box.LostFocus += OnPasswordBoxLostFocus;

                // НОВОЕ: Инициализируем watermark
                UpdateWatermarkVisibility(box);
            }
        }

        // НОВЫЕ МЕТОДЫ для обработки watermark
        private static void OnPasswordBoxGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;
            UpdateWatermarkVisibility(box);
        }

        private static void OnPasswordBoxLostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;
            UpdateWatermarkVisibility(box);
        }

        private static void UpdateWatermarkVisibility(PasswordBox box)
        {
            if (box != null)
            {
                bool shouldShowWatermark = string.IsNullOrEmpty(box.Password) && !box.IsFocused;
                SetShowWatermark(box, shouldShowWatermark);
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            SetUpdatingPassword(box, true);
            SetBoundPassword(box, box.Password);
            SetUpdatingPassword(box, false);

            // НОВОЕ: Обновляем видимость watermark при изменении пароля
            UpdateWatermarkVisibility(box);
        }
    }
}