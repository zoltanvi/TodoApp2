using Modules.Notes.Repositories.Models;

namespace Modules.Notes.Repositories;

public interface INotesRepository
{
    Note AddNote(Note note);
    void DeleteNote(Note note);
    List<Note> GetActiveNotes();
    void RestoreNote(Note note);
    void UpdateNoteContent(Note updatedNote);
    void UpdateNoteListOrders(List<Note> notes);
}