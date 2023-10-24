﻿namespace TodoApp2.Core
{
    public class TaskPageSettings : SettingsBase
    {
        public bool InsertOrderReversed { get; set; }
        public bool ForceTaskOrderByState { get; set; }
        public bool ExitEditOnFocusLost { get; set; }
        public bool SaveOnEnter { get; set; } = true;
        public bool PlaySoundOnTaskIsDoneChange { get; set; } = true;
        public bool TaskListMargin { get; set; } = true;
        public bool FormattedPasteEnabled { get; set; } = true;
    }
}
