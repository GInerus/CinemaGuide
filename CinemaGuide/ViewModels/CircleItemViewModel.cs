using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CinemaGuide.Models;


namespace CinemaGuide.ViewModels
{
    public class CircleItemViewModel : BaseViewModel
    {
        public CircleItem Item { get; }

        public CircleItemViewModel(CircleItem item)
        {
            Item = item;
        }

        public string Login => Item.Login;

        public string FirstCharacterOfLogin
            => string.IsNullOrEmpty(Login) ? "?" : Login[0].ToString();

        public bool HasImage => File.Exists(Item.ImagePath);

        public Brush BackgroundBrush
        {
            get
            {
                if (HasImage)
                {
                    return new ImageBrush(new BitmapImage(new Uri(Item.ImagePath)))
                    {
                        Stretch = Stretch.UniformToFill,
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center
                    };
                }

                return new SolidColorBrush(GenerateColorFromLogin(Login));
            }
        }

        private Color GenerateColorFromLogin(string login)
        {
            string[] colors =
            {
            "#FF0000","#FF8000","#FFFF00",
            "#80FF00","#00FF00","#00FF80",
            "#00FFFF","#0080FF","#0000FF",
            "#8000FF","#FF00FF","#FF0080"
        };

            int hash = Math.Abs(login.GetHashCode());
            string hex = colors[hash % colors.Length];

            return (Color)ColorConverter.ConvertFromString(hex);
        }
    }

}
