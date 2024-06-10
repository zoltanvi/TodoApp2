using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Models;
using Modules.Common;
using Modules.Notes.Repositories;
using Modules.Notes.Repositories.Models;
using System;

namespace TodoApp2.DefaultData;

public class DefaultDataCreator
{
    private readonly ICategoriesDbInfoRepository _categoriesDbInfoRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly INotesDbInfoRepository _notesDbInfoRepository;
    private readonly INotesRepository _notesRepository;

    public DefaultDataCreator(
        ICategoriesDbInfoRepository categoriesDbInfoRepository,
        ICategoriesRepository categoriesRepository,
        INotesDbInfoRepository notesDbInfoRepository,
        INotesRepository notesRepository)
    {
        ArgumentNullException.ThrowIfNull(categoriesDbInfoRepository);
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(notesDbInfoRepository);
        ArgumentNullException.ThrowIfNull(notesRepository);

        _categoriesDbInfoRepository = categoriesDbInfoRepository;
        _categoriesRepository = categoriesRepository;
        _notesDbInfoRepository = notesDbInfoRepository;
        _notesRepository = notesRepository;
    }

    public void CreateDefaultsIfNeeded()
    {
        CreateDefaultCategory();
        CreateDefaultNote();
    }

    private void CreateDefaultCategory()
    {
        if (!_categoriesDbInfoRepository.Initialized)
        {
            _categoriesDbInfoRepository.Initialized = true;

            _categoriesRepository.AddCategory(new Category { Name = Constants.CategoryName.Today });

            _categoriesRepository.AddCategory(new Category
            {
                Id = Constants.RecycleBinCategoryId,
                Name = Constants.CategoryName.RecycleBin,
                ListOrder = int.MinValue,
            });

        }
    }

    private void CreateDefaultNote()
    {
        if (!_notesDbInfoRepository.Initialized)
        {
            _notesDbInfoRepository.Initialized = true;

            _notesRepository.AddNote(new Note { Title = "Empty note", });
        }
    }
}
