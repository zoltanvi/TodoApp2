using System;
using System.Windows.Media;

namespace TodoApp2.Core;

public class MediaPlayerService
{
    MediaPlayer _mediaPlayer;
    MediaPlayer _mediaPlayerReverse;

    private TaskPageSettings TaskPageSettings => IoC.AppViewModel.AppSettings.TaskPageSettings;

    public MediaPlayerService()
    {
        _mediaPlayer = new MediaPlayer();
        double volume = _mediaPlayer.Volume;
        _mediaPlayer.Volume = 0;
        _mediaPlayer.Open(new Uri("Sounds/click.mp3", UriKind.Relative));

        _mediaPlayerReverse = new MediaPlayer();
        _mediaPlayerReverse.Volume = 0;
        _mediaPlayerReverse.Open(new Uri("Sounds/click_reverse.mp3", UriKind.Relative));
        
        _mediaPlayer.Volume = volume;
        _mediaPlayerReverse.Volume = volume;
    }

    public void PlayClick()
    {
        if (TaskPageSettings.PlaySoundOnTaskIsDoneChange)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Play();
        }
    }

    public void PlayClickReverse()
    {
        if (TaskPageSettings.PlaySoundOnTaskIsDoneChange)
        {
            _mediaPlayerReverse.Stop();
            _mediaPlayerReverse.Play();
        }
    }
}
