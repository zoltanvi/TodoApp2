using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.ViewModel;
using System;
using System.Windows.Threading;

namespace TodoApp2.Core;

public class NotePageViewModel : BaseViewModel
{
    private AppViewModel _appViewModel;
    private NoteListService _noteListService;
    private DispatcherTimer _timer;
    private bool _saved;
    private bool _initialized;

    public bool IsNoteExists { get; private set; }

    public NotePageViewModel(
        AppViewModel appViewModel, 
        NoteListService noteListService)
    {
        _appViewModel = appViewModel;
        _noteListService = noteListService;
        _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 2) };
        _timer.Tick += TimerOnTick;

        IsNoteExists = _noteListService.ActiveNote != null;

        Mediator.Register(OnNoteChanged, ViewModelMessages.NoteChanged);
    }

    private void OnNoteChanged(object obj)
    {
        IsNoteExists = _noteListService.ActiveNote != null;
    }

    private void TimerOnTick(object sender, EventArgs e)
    {
        if (!_saved)
        {
            SaveNoteContent();
        }
    }

    public void SaveNoteContent()
    {
        _saved = true;
        _timer.Stop();

        _noteListService.SaveNoteContent();
        
        _appViewModel.SaveIconVisible = !_appViewModel.SaveIconVisible;
    }

    public void NoteContentChanged()
    {
        if (_initialized)
        {
            _saved = false;
            _timer.Stop();
            _timer.Start();
        }
        else
        {
            _initialized = true;
        }
    }
}
