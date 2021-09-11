﻿using GongSolutions.Wpf.DragDrop;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Locates view models from the IoC for use in binding in XAML files
    /// </summary>
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance { get; } = new ViewModelLocator();
        
        public static ApplicationViewModel ApplicationViewModel => IoC.ApplicationViewModel;
        public static OverlayPageService OverlayPageService => IoC.OverlayPageService;
        public static CategoryListService CategoryListService => IoC.CategoryListService;
        public static TaskListService TaskListService => IoC.TaskListService;
        public static SessionManager SessionManager => IoC.SessionManager;
        public static MessageService MessageService => IoC.MessageService;

        public static ColorListProvider ColorListProvider { get; } = new ColorListProvider();

        public static AccentColorProvider AccentColorProvider { get; } = new AccentColorProvider();
        public static CustomDropHandler CustomDropHandler { get; } = new CustomDropHandler();
    }
}