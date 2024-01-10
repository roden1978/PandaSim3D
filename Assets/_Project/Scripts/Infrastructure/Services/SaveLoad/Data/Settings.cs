using System;

[Serializable]
public class Settings
{
    public SoundSettings SoundSettings;
    public StaticSoundSettings StaticSoundSettings;

    public Settings()
    {
        SoundSettings = new SoundSettings();
        StaticSoundSettings = new StaticSoundSettings();
    }
}