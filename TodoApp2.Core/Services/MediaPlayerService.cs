using System;
using System.Windows.Media;

namespace TodoApp2.Core
{
    public class MediaPlayerService
    {
        MediaPlayer m_MediaPlayer;
        MediaPlayer m_MediaPlayerReverse;

        public MediaPlayerService()
        {
            m_MediaPlayer = new MediaPlayer();
            double volume = m_MediaPlayer.Volume;
            m_MediaPlayer.Volume = 0;
            m_MediaPlayer.Open(new Uri("Sounds/click.mp3", UriKind.Relative));
            m_MediaPlayerReverse = new MediaPlayer();
            m_MediaPlayerReverse.Open(new Uri("Sounds/click_reverse.mp3", UriKind.Relative));
            m_MediaPlayer.Volume = volume;
        }

        public void PlayClick()
        {
            if (IoC.AppViewModel.ApplicationSettings.PlaySoundOnTaskIsDoneChange)
            {
                m_MediaPlayer.Stop();
                m_MediaPlayer.Play();
            }
        }

        public void PlayClickReverse()
        {
            if (IoC.AppViewModel.ApplicationSettings.PlaySoundOnTaskIsDoneChange)
            {
                m_MediaPlayerReverse.Stop();
                m_MediaPlayerReverse.Play();
            }
        }
    }
}
