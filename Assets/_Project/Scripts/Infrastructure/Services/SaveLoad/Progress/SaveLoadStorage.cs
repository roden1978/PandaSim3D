using System.Collections.Generic;
using UnityEngine;

namespace Services.SaveLoad.PlayerProgress
{
    public class SaveLoadStorage : ISaveLoadStorage
    {
        private readonly List<ISavedProgress> _savers = new();
        private readonly List<ISavedProgressReader> _readers = new();

        public IEnumerable<ISavedProgress> Savers => _savers;
        public IEnumerable<ISavedProgressReader> Readers => _readers;

        public void ClearAll()
        {
            _savers.Clear();
            _readers.Clear();
        }

        public void ClearGameObjectsType()
        {
            _savers.RemoveAll(x => x.GetType().BaseType!.Name == "MonoBehaviour");
        }

        public void RegisterInSaveLoadRepositories(GameObject registeredGameObject)
        {
            foreach (ISavedProgress savedProgress in registeredGameObject
                         .GetComponentsInChildren<ISavedProgress>())
            {
                if (Has(savedProgress)) continue;
                Debug.Log($"Saved progress go {savedProgress}");
                AddProgressWriter(savedProgress);
                AddProgressReader(savedProgress);
            }
        }

        private bool Has(ISavedProgress savedObject)
        {
            return _savers.Contains(savedObject);
        }

        public void RegisterInSaveLoadRepositories(ISavedProgress savedProgress)
        {
            if (Has(savedProgress)) return;
            Debug.Log($"Saved progress {savedProgress}");
            AddProgressWriter(savedProgress);
            AddProgressReader(savedProgress);
        }

        private void AddProgressWriter(ISavedProgress progressWriter)
        {
            _savers.Add(progressWriter);
        }

        private void AddProgressReader(ISavedProgressReader progressReader)
        {
            _readers.Add(progressReader);
        }
    }
}