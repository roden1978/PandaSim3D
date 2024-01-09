using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Egg : MonoBehaviour, ISavedProgress, IPointerClickHandler
{
    [SerializeField][Range(1, 5)]
    private int _touchesCount;

    private ISaveLoadService _saveLoadService;
    private MeshRenderer _meshRenderer;
    private int _touches;
    private Color _startColor;
    private Color _endColor;
    private Renderer _renderer;

    [Inject]
    public void Construct(ISaveLoadService saveLoadService)
    {
        _saveLoadService = saveLoadService;
        /*_renderer = gameObject.GetComponent<Renderer>();
        _startColor = _renderer.sharedMaterial.color;
        _endColor = new Color(_startColor.r, _startColor.g, _startColor.b, 0);*/
    }
    public void LoadProgress(PlayerProgress playerProgress)
    {
        
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        playerProgress.PlayerState.FirstStartGame = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Touch egg");
        /*if (_touchesCount == 0)
        {
            _saveLoadService.SaveProgress();
            Destroy(gameObject);
        }

        Color lerpColor = Color.Lerp(_startColor, _endColor, 1 / _touchesCount);
        _renderer.sharedMaterial.color = lerpColor;
        _touchesCount -= 1;*/
    }
}
