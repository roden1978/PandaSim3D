using PlayerScripts;
using UnityEngine;

namespace GameObjectsScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Transform _endWayPointTransform;
        [SerializeField] private float _speed;
        private Rigidbody2D _platformRigidbody2D;
        private Rigidbody2D _playerRigidbody2D;
        private Vector3[] _wayPoints;
        private Vector3 _endWayPointLocalPosition;
        private Vector3 _startPosition;
        private Vector3 _endWayPointPosition;
        private int _wayPointsCount;
        private float _startTime;
        private float _journeyLength;

        private void Start()
        {
            Vector3 startPoint = transform.position;
            _startPosition = startPoint;
            _endWayPointLocalPosition = _endWayPointTransform.localPosition;
            _endWayPointPosition = _endWayPointTransform.position;
            _platformRigidbody2D = GetComponent<Rigidbody2D>();
            _wayPoints = new []{startPoint + _endWayPointLocalPosition, startPoint};
            
            _startTime = Time.time;

            _journeyLength = Vector3.Distance(startPoint, _wayPoints[0]);
        }

        private void Update()
        {
            Moving();
        }

        private void Moving()
        {
            bool reachedEndOfPath = Vector3.Distance(transform.position, _wayPoints[_wayPointsCount]) < .5;

            if (reachedEndOfPath)
            {
                _wayPointsCount++;
                _startPosition = transform.position;
                if (_wayPointsCount >= _wayPoints.Length)
                    _wayPointsCount = 0;
                _journeyLength = Vector3.Distance(_startPosition, _wayPoints[_wayPointsCount]);
                _startTime = Time.time;
            }
            float distCovered = (Time.time - _startTime) * _speed;

            float fractionOfJourney = distCovered / _journeyLength;
            Vector3 position =
                Vector3.Lerp(_startPosition, _wayPoints[_wayPointsCount], fractionOfJourney);
            _platformRigidbody2D.MovePosition(position);
            _endWayPointTransform.position = _endWayPointPosition;
            
            if (_playerRigidbody2D != null)
                _playerRigidbody2D.velocity += _platformRigidbody2D.velocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                _playerRigidbody2D = player.GetComponent<Rigidbody2D>();
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            _playerRigidbody2D = null;
        }

        private void OnDrawGizmos()
        {
            Vector3 position = _endWayPointTransform.position;
            Gizmos.DrawLine(transform.position, position);
            Gizmos.DrawWireSphere(position, 1f);
        }
    }
}
