using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Services.Pool
{
    public class Pool
    {
        public int Count => _repository.Count;
        public int Capacity { get; }
        public GameObject PooledObject { get; }

        private readonly PooledObjectTypes _type;
        private readonly Queue<GameObject> _repository = new ();

        public Pool(PooledObjectTypes type, GameObject pooledObject, int capacity)
        {
            PooledObject = pooledObject;
            Capacity = capacity;
            _type = type;
        }

        public GameObject GetPooledObject()
        {
            return _repository.Dequeue();
        }

        public GameObject GetFirst()
        {
            return _repository.Peek();
        }

        public void SetPooledObject(GameObject pooledObject)
        {
            _repository.Enqueue(pooledObject);
        }

        public void ClearPool()
        {
            Debug.Log($"Clear Pool {Enum.GetName(typeof(PooledObjectTypes), _type)}");
            foreach (GameObject go in _repository)
            {
                Debug.Log($"Gameobject name {go.name}");
                Object.Destroy(go);
            }
            _repository.Clear();
        }
    }
}