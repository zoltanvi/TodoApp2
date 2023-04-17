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
            m_MediaPlayer.Open(new Uri("Sounds/click.mp3", UriKind.Relative));
            m_MediaPlayerReverse = new MediaPlayer();
            m_MediaPlayerReverse.Open(new Uri("Sounds/click_reverse.mp3", UriKind.Relative));
        }

        public void PlayClick()
        {
            if (IoC.ApplicationViewModel.ApplicationSettings.PlaySoundOnTaskIsDoneChange)
            {
                m_MediaPlayer.Stop();
                m_MediaPlayer.Play();
            }
        }

        public void PlayClickReverse()
        {
            if (IoC.ApplicationViewModel.ApplicationSettings.PlaySoundOnTaskIsDoneChange)
            {
                m_MediaPlayerReverse.Stop();
                m_MediaPlayerReverse.Play();
            }
        }
    }
}
