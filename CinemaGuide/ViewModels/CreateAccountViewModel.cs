using CinemaGuide.Data;
using CinemaGuide.Helpers;
using CinemaGuide.Models;
using CinemaGuide.Views.UserControls;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CinemaGuide.ViewModels
{
    public class CreateAccountViewModel : BaseViewModel
    {
        private CircleItem _circleItem;
        public CircleItem CircleItem
        {
            get => _circleItem;
            set { _circleItem = value; OnPropertyChanged(); OnPropertyChanged(nameof(BackgroundBrush)); OnPropertyChanged(nameof(FirstCharacter)); }
        }

        public Brush BackgroundBrush => CircleItem?.Color ?? Brushes.Gray;
        public string FirstCharacter => CircleItem?.FirstCharacterOfLogin ?? "?";

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                UpdateCircle();
            }
        }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }

        private DateTime? _birthDate;
        public DateTime? BirthDate { get => _birthDate; set { _birthDate = value; OnPropertyChanged(); } }

        private string _avatarPath;
        public string AvatarPath
        {
            get => _avatarPath;
            set { _avatarPath = value; OnPropertyChanged(); OnPropertyChanged(nameof(AvatarVisibility)); }
        }

        public Visibility AvatarVisibility => string.IsNullOrEmpty(AvatarPath) ? Visibility.Collapsed : Visibility.Visible;

        public ICommand RegisterCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ChooseAvatarCommand { get; }

        public CreateAccountViewModel()
        {
            CircleItem = new CircleItem { Login = "?" };
            RegisterCommand = new RelayCommand(Register);
            BackCommand = new RelayCommand(BackToLast);
            ChooseAvatarCommand = new RelayCommand(ChooseAvatar);
        }

        private void UpdateCircle()
        {
            if (string.IsNullOrEmpty(Username))
            {
                CircleItem.Login = "?";
                CircleItem.ImagePath = null;
            }
            else
            {
                using var db = new AppDbContext();
                var user = db.Users.FirstOrDefault(u => u.Username == Username);
                if (user != null)
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string path = Path.Combine(baseDir, "avatars", user.AvatarPath ?? "");
                    CircleItem.ImagePath = File.Exists(path) ? path : null;
                }
                else
                {
                    CircleItem.ImagePath = null;
                }
                CircleItem.Login = Username;
            }

            OnPropertyChanged(nameof(BackgroundBrush));
            OnPropertyChanged(nameof(FirstCharacter));
        }

        private void ChooseAvatar(object parameter)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg" };
            if (dlg.ShowDialog() == true)
                AvatarPath = dlg.FileName;
        }

        private void Register(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword) ||
                BirthDate == null)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            using var db = new AppDbContext();
            if (db.Users.Any(u => u.Username == Username))
            {
                MessageBox.Show("Пользователь с таким именем уже существует");
                return;
            }

            var user = new User
            {
                Username = Username,
                PasswordHash = PasswordHasher.HashPassword(Password),
                BirthDate = BirthDate.Value.ToString("yyyy-MM-dd"),
                AvatarPath = AvatarPath
            };

            db.Users.Add(user);
            db.SaveChanges();

            MessageBox.Show("Регистрация успешна!");
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new AuthUserControl();
        }

        private void BackToLast(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new AuthUserControl();
        }
    }
}
