using Infrastructure;
using Infrastructure.AssetManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PointerListener _restart;
    [SerializeField] private PointerListener _language;
    [SerializeField] private PointerListener _soundSettings;
    [SerializeField] private PointerListener _about;
    [SerializeField] private PointerListener _exit;
    [SerializeField] private PointerListener _continue;
    [SerializeField] private Canvas _aboutPanel;
    [SerializeField] private Canvas _soundSettingsPanel;
    [SerializeField] private Canvas _languagePanel;


    private ISceneLoader _sceneLoader;
    private ISaveLoadService _saveLoadService;
    private IPersistentProgress _persistentProgress;

    [Inject]
    public void Construct(IPersistentProgress persistentProgress, ISceneLoader sceneLoader,
        ISaveLoadService saveLoadService)
    {
        _sceneLoader = sceneLoader;
        _saveLoadService = saveLoadService;
        _persistentProgress = persistentProgress;
    }

    private void OnEnable()
    {
        _restart.Click += OnRestartButton;
        _language.Click += OnLanguageButton;
        _soundSettings.Click += OnSoundSettingsButton;
        _about.Click += OnAboutButton;
        _exit.Click += OnExitButton;
        _continue.Click += OnContinueButton;
    }

    private void OnDisable()
    {
        _restart.Click -= OnRestartButton;
        _language.Click -= OnLanguageButton;
        _soundSettings.Click -= OnSoundSettingsButton;
        _about.Click -= OnAboutButton;
        _exit.Click -= OnExitButton;
        _continue.Click -= OnContinueButton;
    }

    private void OnContinueButton(PointerEventData obj)
    {
        HideMenu();
    }

    private void OnLanguageButton(PointerEventData obj)
    {
        HideMenu();
        _languagePanel.gameObject.SetActive(true);
    }

    private void OnRestartButton(PointerEventData data)
    {
        _saveLoadService.Delete();
        _persistentProgress.PlayerProgress = new PlayerProgress();
        _persistentProgress.Settings = new Settings();
        _sceneLoader.LoadScene(AssetPaths.CurtainSceneName);
    }

    private void OnExitButton(PointerEventData data)
    {
        _saveLoadService.SaveProgress();
        HideMenu();
        Application.Quit();
    }

    private void OnAboutButton(PointerEventData data)
    {
        HideMenu();
        _aboutPanel.gameObject.SetActive(true);
    }

    private void OnSoundSettingsButton(PointerEventData data)
    {
        HideMenu();
        _soundSettingsPanel.gameObject.SetActive(true);
    }

    private void HideMenu()
    {
        gameObject.SetActive(false);
    }
}