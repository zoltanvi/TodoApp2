using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    public class VSBlueColorScheme : BaseColorScheme
    {
        public override void ChangeColors()
        {
            SetColor("AppBorderColor", "#FF0000");
            SetColor("ThemeLighterColor", "#FF00FF");
        }
    }
}
