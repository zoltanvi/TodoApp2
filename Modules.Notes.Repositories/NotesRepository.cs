using Modules.Notes.Repositories.Models;

namespace Modules.Notes.Repositories;

public class NotesRepository : INotesRepository
{
    private readonly NotesDbContext _context;

    public NotesRepository(NotesDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public Note AddNote(Note note)
    {
        note.CreationDate = DateTime.Now;
        note.ModificationDate = DateTime.Now;

        _context.Notes.Add(note);
        _context.SaveChanges();

        return note;
    }

    public List<Note> GetActiveNotes()
    {
        return _context.Notes
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.ListOrder)
            .ToList();
    }

    public void UpdateNoteContent(Note updatedNote)
    {
        var dbNote = _context.Notes.Find(updatedNote.Id);
        ArgumentNullException.ThrowIfNull(dbNote);

        dbNote.Content = updatedNote.Content;
        dbNote.ModificationDate = DateTime.Now;

        _context.SaveChanges();
    }

    public void DeleteNote(Note note)
    {
        var dbNote = _context.Notes.Find(note.Id);
        ArgumentNullException.ThrowIfNull(dbNote);

        dbNote.IsDeleted = true;
        dbNote.ListOrder = -1;
        _context.SaveChanges();
    }

    public void RestoreNote(Note note)
    {
        var dbNote = _context.Notes.Find(note.Id);
        ArgumentNullException.ThrowIfNull(dbNote);

        dbNote.IsDeleted = false;
        dbNote.ListOrder = 0;
        _context.SaveChanges();
    }

    public void UpdateNoteListOrders(List<Note> notes)
    {
        foreach (var updatedNote in notes)
        {
            var dbNote = _context.Notes.Find(updatedNote.Id);
            ArgumentNullException.ThrowIfNull(dbNote);

            if (dbNote != null)
            {
                dbNote.ListOrder = updatedNote.ListOrder;
            }
        }

        _context.SaveChanges();
    }
}
