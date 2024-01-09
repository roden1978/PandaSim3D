using UnityEngine;

namespace GameObjectsScripts
{
    public class FloatingMove : MonoBehaviour
    {
        [SerializeField] private float _height;
        [SerializeField] protected float _speed;
        private float _startY;
        private void Start()
        {
            _startY = transform.position.y;
        }
        
        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if(_speed > 0)
            {
                Vector3 position = transform.position;
                float newY = _startY + _height * Mathf.Sin(Time.time * _speed);
                transform.position = new Vector3(position.x, newY, 0);
            }
        }
    }
}