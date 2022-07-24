using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    public abstract class BaseColorScheme : IColorScheme
    {
        public abstract void ChangeColors();

        protected void SetColor(string resourceName, string colorCode)
        {
            if (Application.Current.Resources.Contains(resourceName))
            {
                Application.Current.Resources[resourceName] = ColorConverter.ConvertFromString(colorCode);
            }
        }
    }
}
