using UnityEngine;
using UnityEngine.EventSystems;

public class Meal : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector3 _mousePosition;
    private Vector3 _startPosition;

    private const int LayerMask = 1 << 8;
    private Camera _camera;

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
            transform.position = _startPosition;
            Debug.Log($"Hit to {raycastHit.collider.gameObject.name}");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Ray screenPointToRay = _camera.ScreenPointToRay(eventData.position);
        Vector3 currentPosition = transform.position;
        Vector3 negativeCameraPosition = -_camera.transform.forward;
        float t = Vector3.Dot(currentPosition - screenPointToRay.origin, negativeCameraPosition) /
                  Vector3.Dot(screenPointToRay.direction,negativeCameraPosition);
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