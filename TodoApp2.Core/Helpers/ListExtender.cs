using System.Collections.Generic;

namespace TodoApp2.Core
{
    public static class ListExtender
    {
        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            T obj = list[oldIndex];
            list.Remove(obj);
            list.Insert(newIndex, obj);
        }
    }
}
