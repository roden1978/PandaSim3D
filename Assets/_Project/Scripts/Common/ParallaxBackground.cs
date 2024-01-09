using UnityEngine;

namespace Common
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Vector2 _parallaxEffectMultiplier;
        [SerializeField] private bool _verticalEffect;
        [SerializeField] private bool _horizontalEffect;
        private Transform _cameraTransform;
        private Vector3 _lastCameraPosition;
        private float _textureUnitSizeX;
        private float _textureUnitSizeY;

        private void Start()
        {
            if (Camera.main is { }) _cameraTransform = Camera.main.transform;
            _lastCameraPosition = _cameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            _textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;
            _textureUnitSizeY = sprite.texture.height / sprite.pixelsPerUnit;
        }

        private void LateUpdate()
        {
            Vector3 cameraPosition = _cameraTransform.position;
            Vector3 deltaMovement = cameraPosition - _lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * _parallaxEffectMultiplier.x, deltaMovement.y * _parallaxEffectMultiplier.y,0);
            _lastCameraPosition = cameraPosition;
            if(_horizontalEffect)
            {
                if (Mathf.Abs(_cameraTransform.position.x - transform.position.x) >= _textureUnitSizeX)
                {
                    float offsetX = (_cameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
                    transform.position = new Vector3(_cameraTransform.position.x + offsetX, transform.position.y);
                }
            }

            if(_verticalEffect){
                if (Mathf.Abs(_cameraTransform.position.y - transform.position.y) >= _textureUnitSizeY)
                {
                    float offsetY = (_cameraTransform.position.y - transform.position.y) % _textureUnitSizeY;
                    transform.position = new Vector3(transform.position.x, _cameraTransform.position.y + offsetY);
                }
            }
        }
    }
}
