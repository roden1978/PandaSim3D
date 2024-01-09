using UnityEngine;

namespace PlayerScripts
{
    public class FloatingPlayerSpawnPoint
    {
        private readonly Camera _camera;
        private readonly float _width;
        private readonly float _height;
        public FloatingPlayerSpawnPoint()
        {
            _camera = Camera.main;
            _width = Screen.width / 2.0f;
            _height = Screen.height / 2.0f;
        }

        public Vector3 Value()
        {
            var stageDimensions = _camera.ScreenToWorldPoint( new Vector3(_width, _height, _camera.nearClipPlane ));
            var spawnPointPosition = new Vector3(stageDimensions.x, stageDimensions.y, 0);
            return spawnPointPosition;
        }
    }
}
