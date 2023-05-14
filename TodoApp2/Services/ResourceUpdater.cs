using System.Windows;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2
{
    public class ResourceUpdater : IResourceUpdater
    {
        private static IResourceUpdater m_Instance;

        public static IResourceUpdater Instance => m_Instance ?? (m_Instance = new ResourceUpdater());

        private ResourceUpdater()
        {
        }

        public void UpdateResource(string resourceName, object resourceValue)
        {
            Application.Current.Resources[resourceName] = new SolidColorBrush((Color)resourceValue);

            // Workaround
            if (resourceName == "ForegroundBrush")
            {
                const string foreground = "Foreground";
                Application.Current.Resources[foreground] = resourceValue;
            }

            if (resourceName == "TaskBgBrush")
            {
                Color res = (Color)resourceValue;
                var transparentColor = new Color() { A = 0, R = res.R, G = res.G, B = res.B };

                const string taskBg = "TaskBg";
                const string taskTransparentBg = "TaskTransparentBg";

                Application.Current.Resources[taskBg] = resourceValue;
                Application.Current.Resources[taskTransparentBg] = transparentColor;
            }
        }

        public object GetResourceValue(string resourceName)
        {
            return ((SolidColorBrush)Application.Current.Resources[resourceName]).Color;
        }
    }
}
