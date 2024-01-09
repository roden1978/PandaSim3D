using System;

namespace Data
{
    [Serializable]
    public class Settings
    {
        public SoundSettings SoundSettings;
        public StaticSoundSetting StaticSoundSetting;

        public Settings()
        {
            SoundSettings = new SoundSettings();
            StaticSoundSetting = new StaticSoundSetting();
        }
    }
}