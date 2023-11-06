using TodoApp2.Entity.Model;

namespace TodoApp2.Persistence.Models
{
    public class Setting : EntityModel
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Setting()
        {
        }

        public Setting(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
