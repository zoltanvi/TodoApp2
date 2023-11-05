using System.Collections.Generic;
using System.Linq;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Core.Mappings
{
    internal static class NoteMapping
    {
        public static Note Map(this NoteViewModel vm)
        {
            if (vm == null) return null;

            return new Note
            {
                Id = vm.Id,
                Title = vm.Title,
                ListOrder = vm.ListOrder.ToString(GlobalConstants.ListOrderFormat),
                Content = vm.Content,
                CreationDate = vm.CreationDate,
                ModificationDate = vm.ModificationDate,
                Trashed = vm.Trashed
            };
        }

        public static NoteViewModel Map(this Note note)
        {
            if (note == null) return null;

            return new NoteViewModel
            {
                Id = note.Id,
                Title = note.Title,
                ListOrder = long.Parse(note.ListOrder),
                Content = note.Content,
                CreationDate = note.CreationDate,
                ModificationDate = note.ModificationDate,
                Trashed = note.Trashed
            };
        }


        public static List<Note> Map(this IEnumerable<NoteViewModel> vmList) => 
            vmList.Select(x => x.Map()).ToList();

        public static List<NoteViewModel> Map(this IEnumerable<Note> modelList) => 
            modelList.Select(x => x.Map()).ToList();
    }
}
