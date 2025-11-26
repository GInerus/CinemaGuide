using CinemaGuide.Data;
using System.Linq;
using CinemaGuide.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace CinemaGuide.ViewModels
{
    public class AuthPageViewModel : BaseViewModel
    {
        public ObservableCollection<object> Circles { get; set; }

        public AuthPageViewModel()
        {
            Circles = new ObservableCollection<object>();
            LoadUsersFromDatabase();
        }

        private void LoadUsersFromDatabase()
        {
            using (var db = new AppDbContext())
            {
                var users = db.Users.ToList();

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                foreach (var user in users)
                {
                    // Строим абсолютный путь
                    string imagePath = Path.Combine(baseDir, "avatars", user.AvatarPath ?? "");

                    // Добавляем в список
                    Circles.Add(
                        new CircleItemViewModel(
                            new CircleItem
                            {
                                Login = user.Username,
                                ImagePath = imagePath
                            }
                        )
                    );
                }

                // Добавляем элемент "+"
                Circles.Add(new CreateAccountItemViewModel(new CreateAccountItem()));
            }
        }
    }
}
