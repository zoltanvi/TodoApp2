//using System;
//using System.Collections.Generic;
//using TodoApp2.Material;

//namespace TodoApp2.Core
//{
//    /// <summary>
//    /// The application settings that are stored in the database.
//    /// </summary>
//    ///
//    /// <remarks>
//    /// The properties and their values that are in this class are stored in the
//    /// database as key-value pairs, where the key is the name of the property
//    /// and the value is the value of the property as string.
//    ///
//    /// The reason why the properties are stored as key-value pairs is that
//    /// this way the database model is always the same, but this class is
//    /// extendable with new properties without changing the database.
//    ///
//    /// To add new properties which will automatically be stored in the database:
//    ///   1. Add a new property to this class.
//    ///   2. Extend <see cref="PropertyDescriptors"/> with the new property's name
//    ///      and the corresponding <see cref="IPropertyValueHandler"/> to it's type.
//    /// </remarks>
//    public class ApplicationSettings : BaseViewModel
//    {
//        private readonly IUIScaler _UiScaler;

//        private Dictionary<string, IPropertyValueHandler> PropertyDescriptors { get; }

//        //public bool IsAnyQuickActionEnabled => IsQuickActionsEnabled && (IsQuickActionsReminderEnabled ||
//        //                                                                 IsQuickActionsColorEnabled ||
//        //                                                                 IsQuickActionsBackgroundColorEnabled ||
//        //                                                                 IsQuickActionsBorderColorEnabled ||
//        //                                                                 IsQuickActionsPinEnabled ||
//        //                                                                 IsQuickActionsTrashEnabled);

//        //public int WindowLeftPos { get; set; }
//        //public int WindowTopPos { get; set; }
//        //public int WindowWidth { get; set; }
//        //public int WindowHeight { get; set; }
//        //public bool CategoryTitleVisible { get; set; } = true;
//        //public bool IsCreationDateVisible { get; set; }
//        //public bool IsModificationDateVisible { get; set; }
//        //public Thickness ColorBarThickness { get; set; } = Thickness.Thick;
//        //public FontFamily FontFamily { get; set; } = FontFamily.SegoeUI;
//        //public FontFamily NoteFontFamily { get; set; } = FontFamily.Consolas;
//        //public bool NotePageWordWrap { get; set; }
//        //public int ActiveCategoryId { get; set; }
//        //public int ActiveNoteId { get; set; }


//        //public bool IsAlwaysOnTop { get; set; }
//        //public bool RunAtStartup { get; set; }
//        //public bool RoundedWindowCorners { get; set; } = true;
//        //public bool IsTitleBarDateVisible { get; set; }
//        //public bool IsTrayIconEnabled { get; set; }
//        //public bool CloseSideMenuOnCategoryChange { get; set; } = true;
//        //public string AccentColor { get; set; } = GlobalConstants.ColorName.Transparent;
//        //public string AppBorderColor { get; set; } = GlobalConstants.ColorName.Transparent;
  
//        //public ApplicationPage MainPage { get; set; }
//        //public ApplicationPage SideMenuPage { get; set; }
//        //public double SideMenuWidth { get; set; } // 0 = closed by default
//        //public bool IsSideMenuOpen { get; set; }
        
        
//        //public Theme ActiveTheme { get; set; } = Theme.ExtraDark;
//        //public bool ThemeIsDarkMode { get; set; }
//        //public ThemeStyle ThemeStyle { get; set; }
//        //public string ThemeSeed { get; set; }


//        //public bool IsItemBackgroundVisible { get; set; } = true;
//        //public bool IsItemBorderVisible { get; set; } = true;
//        //public bool IsQuickActionsEnabled { get; set; } = true;
//        //public bool IsQuickActionsCheckboxEnabled { get; set; } = true;
//        //public bool IsQuickActionsReminderEnabled { get; set; } = true;
//        //public bool IsQuickActionsColorEnabled { get; set; } = true;
//        //public bool IsQuickActionsBackgroundColorEnabled { get; set; } = true;
//        //public bool IsQuickActionsBorderColorEnabled { get; set; }
//        //public bool IsQuickActionsPinEnabled { get; set; } = true;
//        //public bool IsQuickActionsTrashEnabled { get; set; } = true;
//        //public double TaskFontSizeDouble { get; set; } = 16;
//        //public TaskSpacing TaskSpacing { get; set; } = TaskSpacing.Normal;
//        //public bool InsertOrderReversed { get; set; }
//        //public bool ForceTaskOrderByState { get; set; }
//        //public bool FocusLostSavesTask { get; set; }
//        //public bool PlaySoundOnTaskIsDoneChange { get; set; } = true;
//        //public bool TaskListHasMargin { get; set; } = true;
//        //public bool IsFormattedPasteEnabled { get; set; } = true;
//        //public bool TextEditorQABold { get; set; } = true;
//        //public bool TextEditorQAItalic { get; set; } = true;
//        //public bool TextEditorQAUnderlined { get; set; } = true;
//        //public bool TextEditorQASmall { get; set; }
//        //public bool TextEditorQAMedium { get; set; }
//        //public bool TextEditorQALarge { get; set; }
//        //public bool TextEditorQAIncrease { get; set; } = true;
//        //public bool TextEditorQADecrease { get; set; } = true;
//        //public bool TextEditorQAMonospace { get; set; } = true;
//        //public bool TextEditorQAReset { get; set; } = true;
//        //public bool TextEditorQATextColor { get; set; } = true;
//        //public bool TextEditorQAAlignLeft { get; set; }
//        //public bool TextEditorQAAlignCenter { get; set; }
//        //public bool TextEditorQAAlignRight { get; set; }
//        //public bool TextEditorQAAlignJustify { get; set; }
//        //public bool SaveOnEnter { get; set; } = true;
//        //public double TitleFontSize { get; set; } = 28;
//        //public FontFamily TitleFontFamily { get; set; } = FontFamily.SegoeUIBold;
//        //public string TitleColor { get; set; } = GlobalConstants.ColorName.Transparent;
//        //public HorizontalAlignment TitleAlignment { get; set; } = HorizontalAlignment.Left;


//        public double Scaling
//        {
//            get => IoC.UIScaler.ScaleValue;
//            set => IoC.UIScaler.SetScaling(value);
//        }

//        public ApplicationSettings(IUIScaler uiScaler)
//        {
//            _UiScaler = uiScaler;

//            // This dictionary describes the data that is stored in database in the Settings table
//            PropertyDescriptors = new Dictionary<string, IPropertyValueHandler>
//            {
//                { nameof(ActiveTheme), new EnumPropertyValueHandler<Theme>() },
//                { nameof(ColorBarThickness), new EnumPropertyValueHandler<Thickness>() },
//                { nameof(TaskSpacing), new EnumPropertyValueHandler<TaskSpacing>() },
//                { nameof(TitleAlignment), new EnumPropertyValueHandler<HorizontalAlignment>() },
//                { nameof(ThemeStyle), new EnumPropertyValueHandler<ThemeStyle>() },
//            };

//            var intProperties = new[]
//            {
//                nameof(WindowLeftPos),
//                nameof(WindowTopPos),
//                nameof(WindowWidth),
//                nameof(WindowHeight),
//                nameof(ActiveCategoryId),
//                nameof(ActiveNoteId)
//            };

//            var boolProperties = new[]
//            {
//                nameof(CategoryTitleVisible),
//                nameof(IsAlwaysOnTop),
//                nameof(IsCreationDateVisible),
//                nameof(IsModificationDateVisible),
//                nameof(IsQuickActionsEnabled),
//                nameof(IsQuickActionsCheckboxEnabled),
//                nameof(IsQuickActionsReminderEnabled),
//                nameof(IsQuickActionsColorEnabled),
//                nameof(IsQuickActionsBackgroundColorEnabled),
//                nameof(IsQuickActionsBorderColorEnabled),
//                nameof(IsQuickActionsPinEnabled),
//                nameof(IsQuickActionsTrashEnabled),
//                nameof(IsItemBackgroundVisible),
//                nameof(IsItemBorderVisible),
//                nameof(RoundedWindowCorners),
//                nameof(IsTitleBarDateVisible),
//                nameof(NotePageWordWrap),
//                nameof(InsertOrderReversed),
//                nameof(ForceTaskOrderByState),
//                nameof(IsColorBarRounded),
//                nameof(IsTrayIconEnabled),
//                nameof(FocusLostSavesTask),
//                nameof(RunAtStartup),
//                nameof(PlaySoundOnTaskIsDoneChange),
//                nameof(TaskListHasMargin),
//                nameof(IsSideMenuOpen),
//                nameof(TaskItemDropShadow),
//                nameof(CloseSideMenuOnCategoryChange),
//                nameof(IsFormattedPasteEnabled),
//                nameof(TextEditorQABold),
//                nameof(TextEditorQAItalic),
//                nameof(TextEditorQAUnderlined),
//                nameof(TextEditorQASmall),
//                nameof(TextEditorQAMedium),
//                nameof(TextEditorQALarge),
//                nameof(TextEditorQAIncrease),
//                nameof(TextEditorQADecrease),
//                nameof(TextEditorQAMonospace),
//                nameof(TextEditorQAReset),
//                nameof(TextEditorQATextColor),
//                nameof(TextEditorQAAlignLeft),
//                nameof(TextEditorQAAlignCenter),
//                nameof(TextEditorQAAlignRight),
//                nameof(TextEditorQAAlignJustify),
//                nameof(SaveOnEnter),
//                nameof(ThemeIsDarkMode)
//            };

//            var stringProperties = new[]
//            {
//                nameof(AccentColor),
//                nameof(AppBorderColor),
//                nameof(TitleColor),
//                nameof(ThemeSeed)
//            };

//            var doubleProperties = new[]
//            {
//                nameof(TaskFontSizeDouble),
//                nameof(Scaling),
//                nameof(SideMenuWidth),
//                nameof(TitleFontSize)
//            };

//            var fontFamilyProperties = new[]
//            {
//                nameof(FontFamily),
//                nameof(NoteFontFamily),
//                nameof(TitleFontFamily),
//            };

//            var applicationPageProperties = new[]
//            {
//                nameof(MainPage),
//                nameof(SideMenuPage)
//            };

//            AddDescriptors(intProperties, new IntegerPropertyValueHandler());
//            AddDescriptors(boolProperties, new BoolPropertyValueHandler());
//            AddDescriptors(stringProperties, new StringPropertyValueHandler());
//            AddDescriptors(doubleProperties, new DoublePropertyValueHandler());
//            AddDescriptors(fontFamilyProperties, new EnumPropertyValueHandler<FontFamily>());
//            AddDescriptors(applicationPageProperties, new EnumPropertyValueHandler<ApplicationPage>());

//            _UiScaler.Zoomed += OnZoomed;
//        }

//        public void TriggerUpdate(string propertyName)
//        {
//            OnPropertyChanged(propertyName);
//        }

//        private void OnZoomed(object sender, EventArgs e)
//        {
//            // This is needed to trigger the font size update (fontSizeScaler)
//            OnPropertyChanged(nameof(TaskFontSizeDouble));
//            OnPropertyChanged(nameof(TitleFontSize));
//        }

//        /// <summary>
//        /// Loads every Settings entry from the provided list.
//        /// </summary>
//        /// <param name="settings">The settings list to load.</param>
//        public void SetSettings(List<Setting> settings)
//        {
//            foreach (Setting entry in settings)
//            {
//                string propertyName = entry.Key;
//                string propertyValue = entry.Value;
//                if (IsPropertyNameValid(propertyName))
//                {
//                    if (PropertyDescriptors.TryGetValue(propertyName, out IPropertyValueHandler valueLoader))
//                    {
//                        valueLoader.SetProperty(this, propertyName, propertyValue);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Returns the settings list created from the properties.
//        /// </summary>
//        /// <returns></returns>
//        public List<Setting> GetSettings()
//        {
//            List<Setting> settings = new List<Setting>();

//            foreach (var propertyDescriptor in PropertyDescriptors)
//            {
//                var propertyName = propertyDescriptor.Key;
//                var propertyLoader = propertyDescriptor.Value;

//                settings.Add(new Setting
//                {
//                    Key = propertyName,
//                    Value = propertyLoader.GetProperty(this, propertyName)
//                });
//            }

//            return settings;
//        }

//        /// <summary>
//        /// Returns a single setting if valid, null otherwise
//        /// </summary>
//        /// <returns></returns>
//        public Setting GetSetting(string propertyName)
//        {
//            if (IsPropertyNameValid(propertyName))
//            {
//                IPropertyValueHandler propertyLoader = PropertyDescriptors[propertyName];
//                return new Setting
//                {
//                    Key = propertyName,
//                    Value = propertyLoader.GetProperty(this, propertyName)
//                };
//            }

//            return null;
//        }

//        /// <summary>
//        /// Checks whether the property name is a valid property or not.
//        /// </summary>
//        /// <param name="propertyName">The property name.</param>
//        /// <returns>Returns true if the name is valid, false otherwise.</returns>
//        private bool IsPropertyNameValid(string propertyName)
//        {
//            return PropertyDescriptors.ContainsKey(propertyName);
//        }

//        private void AddDescriptors(IEnumerable<string> names, IPropertyValueHandler handler)
//        {
//            foreach (var name in names)
//            {
//                PropertyDescriptors.Add(name, handler);
//            }
//        }
//    }
//}