using Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _start;
        [SerializeField] private Button _options;
        [SerializeField] private Button _about;
        [SerializeField] private Button _exit;
        [SerializeField] private About _aboutPanel;
        [SerializeField] private Settings _settingsPanel;
        [SerializeField] private AudioSource _mainMenuMusic;
        private bool _mute;
        private float _volume;

        private void OnEnable()
        {
            _start.onClick.AddListener(OnStartButton);
            _options.onClick.AddListener(OnOptionsButton);
            _about.onClick.AddListener(OnAboutButton);
            _exit.onClick.AddListener(OnExitButton);
        }

        private void Start()
        {
            //IPersistentProgressService persistentProgressService =
           //     ServiceLocator.Container.Single<IPersistentProgressService>();
           //  _volume = persistentProgressService.Settings.SoundSettings.Volume;
          //  _mute = persistentProgressService.Settings.SoundSettings.Mute == 0;
            _settingsPanel.SetMute(_mute);
            _settingsPanel.SetVolume(_volume);
            _settingsPanel.UpdateSettingsControls(_volume, _mute);
            _mainMenuMusic.Play();
        }

        
        private void OnDisable()
        {
            _start.onClick.RemoveListener(OnStartButton);
            _options.onClick.RemoveListener(OnOptionsButton);
            _about.onClick.RemoveListener(OnAboutButton);
            _exit.onClick.RemoveListener(OnExitButton);
        }

        private void OnExitButton()
        {
            Application.Quit();
        }

        private void OnAboutButton()
        {
            _aboutPanel.gameObject.SetActive(true);
        }

        private void OnOptionsButton()
        {
            _settingsPanel.gameObject.SetActive(true);
        }

        private void OnStartButton()
        {
            HideMenu();
            Bootstrapper bootstrapper = FindObjectOfType<Bootstrapper>();
            //bootstrapper.LoadLevelState();
        }

        private void HideMenu()
        {
            gameObject.SetActive(false);
        }
    }
}
