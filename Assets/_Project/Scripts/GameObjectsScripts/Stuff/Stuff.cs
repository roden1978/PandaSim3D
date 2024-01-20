using GameObjectsScripts;
using PlayerScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IStuff : IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler,
    IDragHandler
{
    
}
public class Stuff : MonoBehaviour, IStuff 
{
    [SerializeField] private Item _item;
    public Item Item => _item;

    private const int LayerMask = 1 << 8 | 1 << 6;
    private Vector3 _mousePosition;
    private Vector3 _startPosition;
    private Camera _camera;
    private Plate _plate;

    public void Construct(Plate plate)
    {
        _plate = plate;
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

            IStack[] stacks = raycastHit.transform.GetComponentsInChildren<IStack>(true);
            foreach (IStack stack in stacks)
            {
                stack.Stack(this);
            }

            _plate.RemoveMeal();
        }
        else
        {
            transform.position = _startPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Ray screenPointToRay = _camera.ScreenPointToRay(eventData.position);
        Vector3 currentPosition = transform.position;
        Vector3 negativeCameraPosition = -_camera.transform.forward;
        float t = Vector3.Dot(currentPosition - screenPointToRay.origin, negativeCameraPosition) /
                  Vector3.Dot(screenPointToRay.direction, negativeCameraPosition);
        Vector3 position = screenPointToRay.origin + screenPointToRay.direction * t;

        transform.position = position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //For begin drag effects
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //For end drag effects
    }
}