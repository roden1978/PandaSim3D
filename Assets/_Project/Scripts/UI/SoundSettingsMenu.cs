using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class SoundSettingsMenu : MonoBehaviour
{
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private PointerListener _back;
    [SerializeField] private Toggle _mute;
    [SerializeField] private Slider _volume;
    [SerializeField] private AudioMixerGroup _mixer;

    private const string Master = "MasterVolume";
    private const string Music = "MusicVolume";
    private const string Effects = "EffectsVolume";
    private const string UI = "UIVolume";

    private const float MaxValue = 0;
    private const float MinValue = -80;

    private IPersistentProgress _persistentProgress;
    private ISaveLoadService _saveLoadService;

    [Inject]
    public void Construct(IPersistentProgress persistentProgress, ISaveLoadService saveLoadService)
    {
        _persistentProgress = persistentProgress;
        _saveLoadService = saveLoadService;
    }

    private void OnEnable()
    {
        _back.Click += OnBackButtonClick;
        _mute.onValueChanged.AddListener(OnMuteChanged);
        _volume.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnDisable()
    {
        _back.Click -= OnBackButtonClick;
        _mute.onValueChanged.RemoveListener(OnMuteChanged);
        _volume.onValueChanged.RemoveListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        SetVolume(value);
    }

    public void SetVolume(float value)
    {
        _mixer.audioMixer.SetFloat(Master, Mathf.Lerp(MinValue, MaxValue, value));
    }

    private void OnMuteChanged(bool enable)
    {
        SetMute(enable);
    }

    public void SetMute(bool enable)
    {
        _mixer.audioMixer.SetFloat(Music, enable ? MaxValue : MinValue);
        _mixer.audioMixer.SetFloat(Effects, enable ? MaxValue : MinValue);
        _mixer.audioMixer.SetFloat(UI, enable ? MaxValue : MinValue);
    }

    private void OnBackButtonClick(PointerEventData data)
    {
        SaveSoundSettings();
        HideSettings();
        ShowMainMenu();
    }

    private void SaveSoundSettings()
    {
        _persistentProgress.Settings.SoundSettings.Mute = _mute.isOn ? 0 : 1;
        _persistentProgress.Settings.SoundSettings.Volume = _volume.value;
        _saveLoadService.SaveSettings();
    }

    private void ShowMainMenu()
    {
        _mainMenu.gameObject.SetActive(true);
    }

    private void HideSettings()
    {
        gameObject.SetActive(false);
    }

    public void UpdateSettingsControls(float volume, bool mute)
    {
        _volume.value = volume;
        _mute.isOn = mute;
    }
}