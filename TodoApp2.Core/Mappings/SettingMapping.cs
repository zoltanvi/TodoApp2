//using System.Collections.Generic;
//using System.Linq;
//using PersistenceSetting = TodoApp2.Persistence.Models.Setting;

//namespace TodoApp2.Core.Mappings
//{
//    internal static class SettingMapping
//    {
//        public static PersistenceSetting Map(this Setting vm)
//        {
//            return new PersistenceSetting
//            {
//                Key = vm.Key,
//                Value = vm.Value,
//            };
//        }

//        public static Setting Map(this PersistenceSetting setting)
//        {
//            return new Setting(setting.Key, setting.Value);
//        }

//        public static List<Setting> Map(this List<PersistenceSetting> settingList)
//        {
//            return settingList.Select<PersistenceSetting, Setting>(x => x.Map()).ToList();
//        }
//    }
//}
