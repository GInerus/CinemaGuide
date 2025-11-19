using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CinemaGuide
{
    public class CircleItem
    {
        public Brush Color { get; set; }
        public String Login { get; set; }
        public String FirstCharacterOfLogin 
            => !string.IsNullOrEmpty(Login) ? Login[0].ToString() : "";
    }
}
