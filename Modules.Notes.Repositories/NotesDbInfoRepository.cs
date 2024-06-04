using Modules.Notes.Repositories.Models;

namespace Modules.Notes.Repositories;

public class NotesDbInfoRepository : INotesDbInfoRepository
{
    private readonly NotesDbContext _context;

    public NotesDbInfoRepository(NotesDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public bool Initialized
    {
        get => _context.NotesDbInfo.Any();
        set
        {
            if (!_context.NotesDbInfo.Any())
            {
                _context.NotesDbInfo.Add(new NotesDbInfo { Initialized = true });
                _context.SaveChanges();
            }
        }
    }
}
