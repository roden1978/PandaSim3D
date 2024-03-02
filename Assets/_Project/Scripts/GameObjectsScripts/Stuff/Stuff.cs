using PlayerScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IStuff : IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler,
    IDragHandler
{
}

public class Stuff : MonoBehaviour, IStuff, IPositionAdapter, IRotationAdapter
{
    [SerializeField] private Item _item;
    public Item Item => _item;
    public IStack LastStack => _lastStack;
    public Vector3 StartPosition
    {
        get => _startPosition;
        set => _startPosition = value;
    }

    private const int LayerMask = 1 << 8 | 1 << 6;
    private Vector3 _mousePosition;
    private Vector3 _startPosition;
    private Camera _camera;
    private IStack _lastStack;

    private bool _isCanDrag;

    private IStack[] _stacks;
    private IPositionAdapter _positionAdapter;

    public void Construct(IStack lastStack, IPositionAdapter positionAdapter)
    {
        _lastStack = lastStack;
        _positionAdapter = positionAdapter;
    }

    private void Start()
    {
        _camera = Camera.main;
        _startPosition = transform.position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool hitResult = Physics.Raycast(
            _camera.ScreenPointToRay(eventData.position), out RaycastHit raycastHit, _camera.farClipPlane, LayerMask);

        if (hitResult)
        {
            if (raycastHit.collider.isTrigger)
            {
                Position = _startPosition;
                return;
            }
            
            Debug.Log($"Hit to {raycastHit.collider.gameObject.name}");

            _stacks = raycastHit.transform.GetComponentsInChildren<IStack>(true);
            foreach (IStack stack in _stacks)
            {
                stack.Stack(this);
            }
        }
        else
        {
            Position = _startPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Ray screenPointToRay = _camera.ScreenPointToRay(eventData.position);
        float zPosition = _startPosition.z >= _positionAdapter.Position.z - 1 ? _positionAdapter.Position.z - 1 : _startPosition.z;
        Vector3 currentPosition = new(Position.x, Position.y, zPosition);
        Vector3 negativeCameraPosition = -_camera.transform.forward;
        float t = Vector3.Dot(currentPosition - screenPointToRay.origin, negativeCameraPosition) /
                  Vector3.Dot(screenPointToRay.direction, negativeCameraPosition);
        Vector3 position = screenPointToRay.origin + screenPointToRay.direction * t;

        if (position.y > 0f)
            Position = position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void AddLastStack(IStack stack)
    {
        _lastStack = stack;
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Quaternion Rotation
    {
        get => transform.rotation;
        set => transform.rotation = value;
    }
}