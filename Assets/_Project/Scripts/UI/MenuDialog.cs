using UnityEngine;
using Zenject;

namespace UI
{
    public class MenuDialog : Dialog, IInitializable
    {
        [SerializeField] private SoundSettings _soundSettings;
        private IPersistentProgress _persistentProgress;

        [Inject]
        public void Construct(IPersistentProgress persistentProgress)
        {
            _persistentProgress = persistentProgress;
        }

        public void Initialize()
        {
            float volume = _persistentProgress.Settings.SoundSettings.Volume;
            bool mute = _persistentProgress.Settings.SoundSettings.Mute == 0;
            _soundSettings.SetMute(mute);
            _soundSettings.SetVolume(volume);
            _soundSettings.UpdateSettingsControls(volume, mute);
        }
        
        
    }
}