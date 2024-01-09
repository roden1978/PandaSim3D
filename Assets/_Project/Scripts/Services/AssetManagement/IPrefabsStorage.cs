using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPrefabsStorage
{
    public int Count { get; }
    void Unregister(GameObject component);
    void Register(Type type, GameObject component);
    void RegisterAll(IEnumerable<GameObject> components);
    GameObject Get(Type type);
    IEnumerable<GameObject> GetAll();
    bool TryGet(Type type, out GameObject component);
}
