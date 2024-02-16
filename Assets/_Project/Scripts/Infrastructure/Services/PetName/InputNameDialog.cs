using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InputNameDialog : Dialog, ISavedProgress
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private PointerListener _continue;
    [SerializeField] private CanvasGroup _continueCanvasGroup;
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private TMP_Text _petName;

    private const int MinNameLenght = 3;
    private const int MaxNameLenght = 20;
    private IPersistentProgress _persistentProgress;
    private ISaveLoadService _saveLoadService;
    private Hud _hud;
    private TimersPrincipal _timerPrincipal;

    [Inject]
    public void Construct(IPersistentProgress persistentProgress, ISaveLoadService saveLoadService, Hud hud, TimersPrincipal timersPrincipal)
    {
        _persistentProgress = persistentProgress;
        _saveLoadService = saveLoadService;
        _hud = hud;
        _timerPrincipal = timersPrincipal;
    }

    private void Start()
    {
        _inputField.characterLimit = MaxNameLenght;
    }

    private void OnEnable()
    {
        _continue.Click += OnClickContinueButton;
        _inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(string text)
    {
        Debug.Log($"Value {text}");
        if (text.Length < MinNameLenght)
        {
            UpdateContinueButtonCanvasGroup(.5f, false, false);
            _warningText.text = $"Min name lenght {MinNameLenght} characters!";
        }
        else
        {
            UpdateContinueButtonCanvasGroup(1, true, true);
            _warningText.text = string.Empty;
        }

        _petName.text = text;
        _hud.UpdatePetName(text);
    }

    private void OnDisable()
    {
        _continue.Click -= OnClickContinueButton;
        _inputField.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnClickContinueButton(PointerEventData eventData)
    {
        _timerPrincipal.AddTimersView(_persistentProgress.PlayerProgress.PlayerState.SceneName);
        _timerPrincipal.StartTimers();
        _saveLoadService.SaveProgress();
        Hide();
    }

    private void UpdateContinueButtonCanvasGroup(float alpha, bool interactable, bool blocksRaycast)
    {
        _continueCanvasGroup.DOFade(alpha, .5f);
        _continueCanvasGroup.interactable = interactable;
        _continueCanvasGroup.blocksRaycasts = blocksRaycast;
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        _persistentProgress.PlayerProgress.PlayerState.FirstStartGame = false;
        _persistentProgress.PlayerProgress.PlayerState.PetName = _inputField.text;
    }
}