using System;

namespace TodoApp2.Entity.Info
{
    internal struct PropInfo
    {
        public string ParentTypeName;
        public string PropName;
        public Type Type;
        public bool IsPrimaryKey;
        public string DefaultValue;
    }
}
