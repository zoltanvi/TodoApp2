using Modules.Notes.Repositories;
using Modules.Notes.Repositories.Models;
using System;

namespace TodoApp2.DefaultData;

public class DefaultDataCreator
{
    private readonly INotesDbInfoRepository _notesDbInfoRepository;
    private readonly INotesRepository _notesRepository;

    public DefaultDataCreator(
        INotesDbInfoRepository notesDbInfoRepository,
        INotesRepository notesRepository)
    {
        ArgumentNullException.ThrowIfNull(notesDbInfoRepository);
        ArgumentNullException.ThrowIfNull(notesRepository);

        _notesDbInfoRepository = notesDbInfoRepository;
        _notesRepository = notesRepository;

        CreateDefaultNote();
    }

    public void CreateDefaultsIfNeeded()
    {
        CreateDefaultNote();
    }

    private void CreateDefaultNote()
    {
        if (!_notesDbInfoRepository.Initialized)
        {
            _notesDbInfoRepository.Initialized = true;

            _notesRepository.AddNote(new Note
            {
                Title = "Empty note",
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now
            });
        }
    }
}
