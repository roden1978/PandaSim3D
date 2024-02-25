using System.Collections.Generic;
using UnityEngine;

namespace Services.SaveLoad.PlayerProgress
{
    public interface ISaveLoadStorage
    {
        public IEnumerable<ISavedProgress> Savers { get; }
        public IEnumerable<ISavedProgressReader> Readers { get; }
        void ClearAll();
        void ClearGameObjectsType();
        void RegisterInSaveLoadRepositories(GameObject registeredGameObject);
        void RegisterInSaveLoadRepositories(ISavedProgress savedProgress);
    }
}