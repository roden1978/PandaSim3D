using UnityEngine;
using UnityEngine.UI;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] private Image _image;
    void Update()
    {
        _image.gameObject.transform.position = Input.mousePosition;
    }
}
