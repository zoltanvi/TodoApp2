namespace TodoApp2.Core
{
    public class ThemeViewModel : BaseViewModel
    {
        public Theme Theme { get; set; }
        public string Name { get; set; }
        public string Foreground { get; set; }
        public string SidebarForeground { get; set; }
        public string SidebarBackground { get; set; }
        public string TitleBar { get; set; }
        public string TaskBackground { get; set; }
        public string PageBackground { get; set; }
        public string SidebarSelectionForeground { get; set; }
        public string SidebarSelectionBackground { get; set; }

        public ThemeViewModel(Theme id, string name)
        {
            Theme = id;
            Name = name;
        }
    }
}
