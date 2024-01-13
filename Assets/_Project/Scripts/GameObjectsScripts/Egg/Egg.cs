using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Egg : MonoBehaviour, ISavedProgress, IPointerClickHandler
{
    [SerializeField] [Range(1, 5)] private int _touchesCount;
    private long _reward = 2000;
    private ISaveLoadService _saveLoadService;
    private MeshRenderer _meshRenderer;
    private int _touches;
    private Color _startColor;
    private Color _endColor;
    private Renderer _renderer;

    private bool _firstGameStart;
    private IWalletService _wallet;
    private DialogManager _dialogManager;

    [Inject]
    public void Construct(ISaveLoadService saveLoadService, IWalletService wallet, DialogManager dialogManager)
    {
        _saveLoadService = saveLoadService;
        _wallet = wallet;
        _dialogManager = dialogManager;
        _renderer = gameObject.GetComponentInChildren<MeshRenderer>();
        _startColor = _renderer.materials[0].color;
        _endColor = new Color(_startColor.r, _startColor.g, _startColor.b, 0);
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        _firstGameStart = playerProgress.PlayerState.FirstStartGame;
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        if (false == _firstGameStart)
            playerProgress.PlayerState.FirstStartGame = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Touch egg");
        if (_touchesCount <= 1)
        {
            _firstGameStart = false;
            _wallet.AddAmount(CurrencyType.Coins, _reward);
            _saveLoadService.SaveProgress();
            _dialogManager.ShowDialog<InputNameDialog>();
            Destroy(gameObject);
        }

        _renderer.materials[0].color =
            new Color(_startColor.r, _startColor.g, _startColor.b, 1 - (float)1 / _touchesCount);
        _touchesCount -= 1;
    }
}