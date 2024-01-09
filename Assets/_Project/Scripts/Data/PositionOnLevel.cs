using System;

namespace Data
{
    [Serializable]
    public class PositionOnLevel
    {
        public Vector3Data Position;
        public string SceneName;
        public int SceneIndex;

        public PositionOnLevel(Vector3Data position, string sceneName)
        {
            Position = position;
            SceneName = sceneName;
        }
    }
}