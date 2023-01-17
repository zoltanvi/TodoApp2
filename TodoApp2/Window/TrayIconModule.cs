using System;
using System.Windows;
using System.Windows.Forms;

namespace TodoApp2
{
    internal class TrayIconModule : IDisposable
    {
        private Window m_Window;
        private NotifyIcon m_NotifyIcon;
        private bool m_IsEnabled;
        private bool m_IsVisible;
        private ContextMenu m_ContextMenu;

        public bool IsEnabled
        {
            get => m_IsEnabled;
            set
            {
                m_IsEnabled = value;
                m_NotifyIcon.Visible = m_IsEnabled;
            }
        }

        public TrayIconModule(Window window)
        {
            m_Window = window;

            m_NotifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("Images\\Tray.ico"),
                Visible = false,
                Text = "TodoApp2"
            };

            m_NotifyIcon.MouseClick += OnNotifyIconMouseClick;

            CreateContextMenu();
        }

        private void OnNotifyIconMouseClick(object sender, MouseEventArgs e)
        {
            if (IsEnabled && e.Button == MouseButtons.Left)
            {
                if (m_IsVisible)
                {
                    MinimizeToTray();
                }
                else
                {
                    ShowWindow();
                }
            }
        }

        public void MinimizeToTray()
        {
            if (IsEnabled)
            {
                m_Window.Hide();
                m_IsVisible = false;
            }
        }

        private void CreateContextMenu()
        {
            m_ContextMenu = new ContextMenu();

            MenuItem[] menuItems = new MenuItem[]
            {
                new MenuItem("Show", OnShowClicked),
                new MenuItem("Exit", OnExitClicked),
            };

            m_ContextMenu.MenuItems.AddRange(menuItems);

            m_NotifyIcon.ContextMenu = m_ContextMenu;
        }

        private void OnShowClicked(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            Dispose();
            m_Window.Close();
        }

        private void ShowWindow()
        {
            if (!m_IsVisible)
            {
                m_Window.Show();
                m_IsVisible = true;
            }

            m_Window.WindowState = WindowState.Normal;
        }

        public void Dispose()
        {
            m_NotifyIcon.Dispose();
        }
    }
}
