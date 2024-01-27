using System;
using UnityEngine;

namespace StaticData
{
    [Serializable]
    public class StuffSpawnData
    {
        public string Id;
        public StuffSpecies StuffSpecies;
        public Vector3 Position;

        public StuffSpawnData(string id, StuffSpecies stuffSpecies, Vector3 position)
        {
            Id = id;
            StuffSpecies = stuffSpecies;
            Position = position;
        }
    }
}