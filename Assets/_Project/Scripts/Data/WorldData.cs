using System;

namespace Data
{
    [Serializable]
    public class WorldData
    {
        public PositionOnLevel PositionOnLevel;

        public WorldData(string sceneName)
        {
            PositionOnLevel = new PositionOnLevel(new Vector3Data(), sceneName);
        }
    }
}