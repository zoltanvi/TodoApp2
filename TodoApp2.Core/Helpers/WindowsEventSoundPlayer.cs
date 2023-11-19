using Microsoft.Win32;
using System;
using System.Media;

namespace TodoApp2.Core
{
    public enum EventSounds
    {
        MailBeep,
    }

    public static class WindowsEventSoundPlayer
    {
        public static void PlayNotificationSound(EventSounds eventSound)
        {
            bool found = false;
            string registryKey = $@"AppEvents\Schemes\Apps\.Default\{(Enum.GetName(typeof(EventSounds), eventSound))}\.Current";
            
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey))
                {
                    if (key != null)
                    {
                        object o = key.GetValue(null); // pass null to get (Default)
                        if (o != null)
                        {
                            SoundPlayer theSound = new SoundPlayer((string)o);
                            theSound.Play();
                            found = true;
                        }
                    }
                }
            }
            catch { }

            if (!found)
            {
                SystemSounds.Beep.Play();
            }
        }
    }
}