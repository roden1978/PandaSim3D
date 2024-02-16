using System.Collections.Generic;
using System.Linq;
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

    private const int LayerMask = 1 << 8 | 1 << 6 ;
    private Vector3 _mousePosition;
    private Vector3 _startPosition;
    private Camera _camera;
    private IStack _lastStack;

    private bool _isCanDrag;

    private IStack[] _stacks;

    public void Construct(IStack lastStack)
    {
        _lastStack = lastStack;
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
            float zPosition = _startPosition.z >= 0 ? -1 : _startPosition.z;
            Vector3 currentPosition = new(Position.x, Position.y, zPosition);
            Vector3 negativeCameraPosition = -_camera.transform.forward;
            float t = Vector3.Dot(currentPosition - screenPointToRay.origin, negativeCameraPosition) /
                      Vector3.Dot(screenPointToRay.direction, negativeCameraPosition);
            Vector3 position = screenPointToRay.origin + screenPointToRay.direction * t;

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