﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TodoApp2.Common;
using TodoApp2.Persistence;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class AppViewModel : BaseViewModel
    {
        private bool _appSettingsLoadedFirstTime;
        private IUIScaler _uiScaler;
        private readonly IAppContext _context;

        private SessionSettings SessionSettings => IoC.AppSettings.SessionSettings;

        /// <summary>
        /// The sliding side menu content page
        /// </summary>
        public ApplicationPage SideMenuPage
        {
            get => SessionSettings.SideMenuPage;
            set => SessionSettings.SideMenuPage = value;
        }

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible { get; set; }

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage MainPage { get; set; }

        /// <summary>
        /// The view model to use for the current page when the Page changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page
        ///       at the time it changes
        /// </summary>
        public IBaseViewModel MainPageViewModel { get; set; }

        /// <summary>
        /// The overlay panel content page
        /// </summary>
        public ApplicationPage OverlayPage { get; private set; }

        /// <summary>
        /// The view model to use for the current overlay page when the OverlayPage changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page
        ///       at the time it changes
        /// </summary>
        public IBaseViewModel OverlayPageViewModel { get; set; }

        /// <summary>
        /// True if the overlay page should be shown
        /// </summary>
        public bool OverlayPageVisible { get; set; }

        /// <summary>
        /// The settings for the whole application
        /// </summary>
        public AppSettings AppSettings
        {
            get => IoC.AppSettings;
            set => IoC.AppSettings = value;
        }

        /// <summary>
        /// The database location.
        /// </summary>
        public string DatabaseLocation => DataAccess.DatabasePath;

        /// <summary>
        /// The value change triggers the animation
        /// </summary>
        public bool SaveIconVisible { get; set; }

        public AppViewModel(IAppContext context, IUIScaler uiScaler)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(uiScaler);

            _context = context;
            _uiScaler = uiScaler;

            // Load the application settings to update the ActiveCategoryId before querying the tasks
            LoadAppSettingsOnce();

            AppSettings.AppWindowSettings.PropertyChanged += CommonSettings_PropertyChanged;
        }

        /// <summary>
        /// Updates the Main Page based on what is stored in the application settings for active category or active note.
        /// </summary>
        public void UpdateMainPage()
        {
            if (SessionSettings.ActiveCategoryId == CommonConstants.RecycleBinCategoryId)
            {
                MainPage = ApplicationPage.RecycleBin;
            }
            else
            {
                // The ActiveCategoryId and the ActiveNoteId must be mutually exclusive,
                // meaning that one or the other is set to -1 at all times, but never both at once.
                MainPage = SessionSettings.ActiveCategoryId != -1
                    ? ApplicationPage.Task
                    : ApplicationPage.Note;
            }
        }

        /// <summary>
        /// Changes the Main Page to the settings page.
        /// </summary>
        public void OpenSettingsPage()
        {
            MainPage = ApplicationPage.Settings;
        }

        /// <summary>
        /// Updates the SideMenu Page based on what is stored in the application settings for active category or active note.
        /// </summary>
        public void UpdateSideMenuPage()
        {
            // The ActiveCategoryId and the ActiveNoteId must be mutually exclusive,
            // meaning that one or the other is set to -1 at all times, but never both at once.
            SideMenuPage = SessionSettings.ActiveCategoryId != -1
                ? ApplicationPage.Category
                : ApplicationPage.NoteList;
        }

        /// <summary>
        /// Navigates the main page to the specified page.
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model to set</param>
        public void GoToPage(ApplicationPage page, IBaseViewModel viewModel = null)
        {
            MainPageViewModel = viewModel;

            // See if page has changed
            bool different = MainPage != page;

            MainPage = page;

            // If the page hasn't changed, fire off notification
            // So pages still update if just the view model has changed
            if (!different)
            {
                //OnPropertyChanged(nameof(MainPage));
            }
        }

        public void OpenPage(ApplicationPage page, TaskViewModel task)
        {
            BaseViewModel viewModel = null;

            switch (page)
            {
                case ApplicationPage.TaskReminder:
                    viewModel = new TaskReminderPageViewModel(_context, task);
                    break;
                case ApplicationPage.ReminderEditor:
                    viewModel = new ReminderEditorPageViewModel(_context, task);
                    break;
                case ApplicationPage.Notification:
                    viewModel = new NotificationPageViewModel(task);
                    break;
                default:
                    throw new ApplicationException("Invalid page.");
            }

            SideMenuVisible = false;

            GoToOverlayPage(page, true, viewModel);
            IoC.OverlayPageService.OverlayBackgroundVisible = true;
        }

        /// <summary>
        /// Closes the overlay page and disposes its viewModel
        /// </summary>
        public void CloseOverlayPage()
        {
            OverlayPage = ApplicationPage.Invalid;
            if (OverlayPageViewModel is IDisposable viewModel)
            {
                viewModel.Dispose();
            }
        }

        public void LoadAppSettingsOnce()
        {
            if (!_appSettingsLoadedFirstTime)
            {
                _appSettingsLoadedFirstTime = true;

                LoadApplicationSettings();
            }
        }

        public void LoadApplicationSettings()
        {
            List<Setting> settingList = _context.Settings.GetAll();
            IoC.AppSettings.PopulateAppSettingsFromList(settingList);

            // Must be set to always check the registry on startup
            IoC.AutoRunService.RunAtStartup = AppSettings.AppWindowSettings.AutoStart;
        }

        public void SaveApplicationSettings()
        {
            List<Setting> settingList = IoC.AppSettings.CreateListFromAppSettings();
            UpdateSettings(settingList);
        }

        private void CommonSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppWindowSettings.AutoStart))
            {
                IoC.AutoRunService.RunAtStartup = AppSettings.AppWindowSettings.AutoStart;
            }
        }

        /// <summary>
        /// Updates all settings entry in the database if exists,
        /// adds the entry if it not exists.
        /// </summary>
        private void UpdateSettings(List<Setting> settings)
        {
            List<Setting> existingSettings = _context.Settings.GetAll();

            Dictionary<string, Setting> existingSettingsMap =
                existingSettings.ToDictionary(settingsModel => settingsModel.Key);

            IEnumerable<Setting> settingsToUpdate = settings
                .Where(s => existingSettingsMap.ContainsKey(s.Key))
                .Where(s => s.Value != existingSettingsMap[s.Key].Value);

            IEnumerable<Setting> settingsToAdd = settings
                .Where(s => !existingSettingsMap.ContainsKey(s.Key));

            if (settingsToAdd.Any())
            {
                _context.Settings.AddRange(settingsToAdd, writeAllProperties: true);
            }

            if (settingsToUpdate.Any())
            {
                _context.Settings.UpdateRange(settingsToUpdate, x => x.Key);
            }
        }

        /// <summary>
        /// Navigates the overlay page to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="visible">Shows the page if true</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page</param>
        private void GoToOverlayPage(ApplicationPage page, bool visible = true, IBaseViewModel viewModel = null)
        {
            // Always hide side menu if we are changing pages
            SideMenuVisible = false;

            // Set the view model
            OverlayPageViewModel = viewModel;

            // See if page has changed
            bool different = OverlayPage != page;

            // Set the current overlay page
            OverlayPage = page;

            // Show or hide the page
            OverlayPageVisible = visible;

            // If the page hasn't changed, fire off notification
            // So pages still update if just the view model has changed
            if (!different)
            {
                OnPropertyChanged(nameof(OverlayPage));
            }
        }
    }
}