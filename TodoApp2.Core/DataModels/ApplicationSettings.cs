using System;
using System.Collections.Generic;
using TodoApp2.Core.Constants;

namespace TodoApp2.Core
{
    /// <summary>
    /// The application settings that are stored in the database.
    /// </summary>
    ///
    /// <remarks>
    /// The properties and their values that are in this class are stored in the
    /// database as key-value pairs, where the key is the name of the property
    /// and the value is the value of the property as string.
    ///
    /// The reason why the properties are stored as key-value pairs is that
    /// this way the database model is always the same, but this class is
    /// extendable with new properties without changing the database.
    ///
    /// To add new properties which will automatically be stored in the database:
    ///   1. Add a new property to this class.
    ///   2. Extend <see cref="PropertyDescriptors"/> with the new property's name
    ///      and the corresponding <see cref="IPropertyValueHandler"/> to it's type.
    /// </remarks>
    public class ApplicationSettings : BaseViewModel
    {
        private readonly IUIScaler _UiScaler;

        private Dictionary<string, IPropertyValueHandler> PropertyDescriptors { get; }

        public bool IsAnyQuickActionEnabled => IsQuickActionsEnabled && (IsQuickActionsReminderEnabled || 
                                                                         IsQuickActionsColorEnabled ||
                                                                         IsQuickActionsBackgroundColorEnabled ||
                                                                         IsQuickActionsBorderColorEnabled ||
                                                                         IsQuickActionsPinEnabled ||
                                                                         IsQuickActionsTrashEnabled);

        public int WindowLeftPos { get; set; }
        public int WindowTopPos { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public int ActiveCategoryId { get; set; }
        public int ActiveNoteId { get; set; }
        public bool CategoryTitleVisible { get; set; } = true;
        public Theme ActiveTheme { get; set; }
        public bool IsAlwaysOnTop { get; set; }
        public bool IsCreationDateVisible { get; set; }
        public bool IsModificationDateVisible { get; set; }
        public Thickness ColorBarThickness { get; set; } = Thickness.Medium;
        public FontFamily FontFamily { get; set; } = FontFamily.SegoeUI;
        public FontFamily NoteFontFamily { get; set; } = FontFamily.Consolas;
        public string AccentColor { get; set; } = GlobalConstants.ColorName.Transparent;
        public string AppBorderColor { get; set; } = GlobalConstants.ColorName.Transparent;
        public bool RoundedWindowCorners { get; set; }
        public bool IsItemBackgroundVisible { get; set; } = true;
        public bool IsItemBorderVisible { get; set; } = true;
        public bool IsQuickActionsEnabled { get; set; } = true;
        public bool IsQuickActionsCheckboxEnabled { get; set; } = true;
        public bool IsQuickActionsReminderEnabled { get; set; } = true;
        public bool IsQuickActionsColorEnabled { get; set; } = true;
        public bool IsQuickActionsBackgroundColorEnabled { get; set; } = true;
        public bool IsQuickActionsBorderColorEnabled { get; set; }
        public bool IsQuickActionsPinEnabled { get; set; } = true;
        public bool IsQuickActionsTrashEnabled { get; set; } = true;
        public double TaskFontSizeDouble { get; set; } = 16;
        public bool IsTitleBarDateVisible { get; set; }
        public bool NotePageWordWrap { get; set; }
        public TaskSpacing TaskSpacing { get; set; }
        public bool InsertOrderReversed { get; set; }
        public bool MoveTaskOnCompletion { get; set; } = true;
        public bool IsColorBarRounded { get; set; } = true;
        public bool IsTrayIconEnabled { get; set; }
        public bool FocusLostSavesTask { get; set; }
        public bool PlaySoundOnTaskIsDoneChange { get; set; } = true;
        public bool RunAtStartup { get; set; }
        public bool TaskListHasMargin { get; set; }
        public bool TaskItemDropShadow { get; set; }
        public ApplicationPage MainPage { get; set; }
        public ApplicationPage SideMenuPage { get; set; }
        public double SideMenuWidth { get; set; } // 0 = closed by default
        public bool IsSideMenuOpen { get; set; }
        public bool CloseSideMenuOnCategoryChange { get; set; } = true;

        public bool TextEditorQABold { get; set; } = true;
        public bool TextEditorQAItalic { get; set; } = true;
        public bool TextEditorQAUnderlined { get; set; } = true;
        public bool TextEditorQASmall { get; set; }
        public bool TextEditorQAMedium { get; set; }
        public bool TextEditorQALarge { get; set; }
        public bool TextEditorQAIncrease { get; set; } = true;
        public bool TextEditorQADecrease { get; set; } = true;
        public bool TextEditorQAMonospace { get; set; } = true;
        public bool TextEditorQAReset { get; set; } = true;
        public bool TextEditorQATextColor { get; set; } = true;


        #region Testing

        public string TextRenderingMode { get; set; }
        public bool TextFormattingMode { get; set; }
        public bool ClearTypeHint { get; set; }

        #endregion

        public double Scaling
        {
            get => IoC.UIScaler.ScaleValue;
            set => IoC.UIScaler.SetScaling(value);
        }

        public ApplicationSettings(IUIScaler uiScaler)
        {
            _UiScaler = uiScaler;

            // This dictionary describes the data that is stored in database in the Settings table
            PropertyDescriptors = new Dictionary<string, IPropertyValueHandler>
            {
                { nameof(WindowLeftPos), PropertyValueHandlers.Integer },
                { nameof(WindowTopPos), PropertyValueHandlers.Integer },
                { nameof(WindowWidth), PropertyValueHandlers.Integer },
                { nameof(WindowHeight), PropertyValueHandlers.Integer },
                { nameof(ActiveCategoryId), PropertyValueHandlers.Integer },
                { nameof(ActiveNoteId), PropertyValueHandlers.Integer },
                { nameof(CategoryTitleVisible), PropertyValueHandlers.Bool },
                { nameof(ActiveTheme), PropertyValueHandlers.Theme },
                { nameof(IsAlwaysOnTop), PropertyValueHandlers.Bool },
                { nameof(IsCreationDateVisible), PropertyValueHandlers.Bool },
                { nameof(IsModificationDateVisible), PropertyValueHandlers.Bool },
                { nameof(IsQuickActionsEnabled), PropertyValueHandlers.Bool },
                { nameof(IsQuickActionsCheckboxEnabled), PropertyValueHandlers.Bool },
                { nameof(IsQuickActionsReminderEnabled), PropertyValueHandlers.Bool },
                { nameof(IsQuickActionsColorEnabled), PropertyValueHandlers.Bool },
                { nameof(IsQuickActionsBackgroundColorEnabled), PropertyValueHandlers.Bool },
                { nameof(IsQuickActionsBorderColorEnabled), PropertyValueHandlers.Bool },
                { nameof(IsQuickActionsPinEnabled), PropertyValueHandlers.Bool },
                { nameof(IsQuickActionsTrashEnabled), PropertyValueHandlers.Bool },
                { nameof(IsItemBackgroundVisible), PropertyValueHandlers.Bool },
                { nameof(IsItemBorderVisible), PropertyValueHandlers.Bool },
                { nameof(ColorBarThickness), PropertyValueHandlers.Thickness },
                { nameof(TaskFontSizeDouble), PropertyValueHandlers.Double },
                { nameof(FontFamily), PropertyValueHandlers.FontFamily },
                { nameof(NoteFontFamily), PropertyValueHandlers.FontFamily },
                { nameof(AccentColor), PropertyValueHandlers.String },
                { nameof(AppBorderColor), PropertyValueHandlers.String },
                { nameof(RoundedWindowCorners), PropertyValueHandlers.Bool },
                { nameof(IsTitleBarDateVisible), PropertyValueHandlers.Bool },
                { nameof(Scaling), PropertyValueHandlers.Double },
                { nameof(NotePageWordWrap), PropertyValueHandlers.Bool },
                { nameof(TextRenderingMode), PropertyValueHandlers.String },
                { nameof(TextFormattingMode), PropertyValueHandlers.Bool },
                { nameof(ClearTypeHint), PropertyValueHandlers.Bool },
                { nameof(TaskSpacing), PropertyValueHandlers.TaskSpacing },
                { nameof(InsertOrderReversed), PropertyValueHandlers.Bool },
                { nameof(MoveTaskOnCompletion), PropertyValueHandlers.Bool },
                { nameof(IsColorBarRounded), PropertyValueHandlers.Bool },
                { nameof(IsTrayIconEnabled), PropertyValueHandlers.Bool },
                { nameof(FocusLostSavesTask), PropertyValueHandlers.Bool },
                { nameof(RunAtStartup), PropertyValueHandlers.Bool },
                { nameof(MainPage), PropertyValueHandlers.ApplicationPage },
                { nameof(SideMenuPage), PropertyValueHandlers.ApplicationPage },
                { nameof(PlaySoundOnTaskIsDoneChange), PropertyValueHandlers.Bool },
                { nameof(TaskListHasMargin), PropertyValueHandlers.Bool },
                { nameof(SideMenuWidth), PropertyValueHandlers.Double },
                { nameof(IsSideMenuOpen), PropertyValueHandlers.Bool },
                { nameof(TaskItemDropShadow), PropertyValueHandlers.Bool },
                { nameof(CloseSideMenuOnCategoryChange), PropertyValueHandlers.Bool },
                { nameof(TextEditorQABold), PropertyValueHandlers.Bool },
                { nameof(TextEditorQAItalic), PropertyValueHandlers.Bool },
                { nameof(TextEditorQAUnderlined), PropertyValueHandlers.Bool },
                { nameof(TextEditorQASmall), PropertyValueHandlers.Bool },
                { nameof(TextEditorQAMedium), PropertyValueHandlers.Bool },
                { nameof(TextEditorQALarge), PropertyValueHandlers.Bool },
                { nameof(TextEditorQAIncrease), PropertyValueHandlers.Bool },
                { nameof(TextEditorQADecrease), PropertyValueHandlers.Bool },
                { nameof(TextEditorQAMonospace), PropertyValueHandlers.Bool },
                { nameof(TextEditorQAReset), PropertyValueHandlers.Bool },
                { nameof(TextEditorQATextColor), PropertyValueHandlers.Bool },
            };

            _UiScaler.Zoomed += OnZoomed;
        }

        private void OnZoomed(object sender, EventArgs e)
        {
            // This is needed to trigger the task font size update (fontSizeScaler)
            OnPropertyChanged(nameof(TaskFontSizeDouble));
        }

        /// <summary>
        /// Loads every Settings entry from the provided list.
        /// </summary>
        /// <param name="settings">The settings list to load.</param>
        public void SetSettings(List<Setting> settings)
        {
            foreach (Setting entry in settings)
            {
                string propertyName = entry.Key;
                string propertyValue = entry.Value;
                if (IsPropertyNameValid(propertyName))
                {
                    if (PropertyDescriptors.TryGetValue(propertyName, out IPropertyValueHandler valueLoader))
                    {
                        valueLoader.SetProperty(this, propertyName, propertyValue);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the settings list created from the properties.
        /// </summary>
        /// <returns></returns>
        public List<Setting> GetSettings()
        {
            List<Setting> settings = new List<Setting>();

            foreach (var propertyDescriptor in PropertyDescriptors)
            {
                var propertyName = propertyDescriptor.Key;
                var propertyLoader = propertyDescriptor.Value;

                settings.Add(new Setting
                {
                    Key = propertyName,
                    Value = propertyLoader.GetProperty(this, propertyName)
                });
            }

            return settings;
        }

        /// <summary>
        /// Returns a single setting if valid, null otherwise
        /// </summary>
        /// <returns></returns>
        public Setting GetSetting(string propertyName)
        {
            if (IsPropertyNameValid(propertyName))
            {
                IPropertyValueHandler propertyLoader = PropertyDescriptors[propertyName];
                return new Setting
                {
                    Key = propertyName,
                    Value = propertyLoader.GetProperty(this, propertyName)
                };
            }

            return null;
        }

        /// <summary>
        /// Checks whether the property name is a valid property or not.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Returns true if the name is valid, false otherwise.</returns>
        private bool IsPropertyNameValid(string propertyName)
        {
            return PropertyDescriptors.ContainsKey(propertyName);
        }
    }
}