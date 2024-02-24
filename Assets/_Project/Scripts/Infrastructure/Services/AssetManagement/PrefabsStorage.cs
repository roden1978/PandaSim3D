using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsStorage : IPrefabsStorage
{
    public int Count => _repository.Count;
    private readonly Dictionary<Type, GameObject> _repository;

    public PrefabsStorage()
    {
        _repository = new Dictionary<Type, GameObject>();
    }

    public GameObject Get(Type type)
    {
        if (!_repository.ContainsKey(type))
            Debug.LogError($"Component with type {type} not found");

        return _repository[type];
    }

    public IEnumerable<GameObject> GetAll()
    {
        return _repository.Values;
    }

    public bool TryGet(Type type, out GameObject component)
    {
        if (!_repository.ContainsKey(type))
        {
            component = default;
            return false;
        }

        component = _repository[type];
        return true;
    }

    public bool Has(Type type)
    {
        return _repository.ContainsKey(type);
    }

    public void Register(Type type, GameObject component)
    {
#if UNITY_EDITOR
        if (Has(type))
            throw new Exception("Type is exist");
#else
        if (Has(type))
            return;
#endif

        _repository[type] = component;
    }

    public void RegisterAll(IEnumerable<GameObject> components)
    {
        foreach (GameObject component in components)
        {
            Type type = components.GetType();
#if UNITY_EDITOR
            if (Has(type))
                throw new Exception("Type is exist");
#else
        if (Has(type))
            return;
#endif
            _repository[type] = component;
        }
    }

    public void Unregister(GameObject component)
    {
        Type type = component.GetType();
        if (_repository.ContainsKey(type))
        {
            _repository.Remove(type);
        }
    }

    public void UnregisterAll()
    {
        _repository.Clear();
    }
}