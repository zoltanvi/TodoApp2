namespace TodoApp2.Core.Mappings
{
    internal static class SettingMapping
    {
        public static TodoApp2.Persistence.Models.Setting Map(this Setting vm)
        {
            return new TodoApp2.Persistence.Models.Setting
            {
                Key = vm.Key,
                Value = vm.Value,
            };
        }

        public static Setting Map(this TodoApp2.Persistence.Models.Setting setting)
        {
            return new Setting(setting.Key, setting.Value);
        }
    }
}
