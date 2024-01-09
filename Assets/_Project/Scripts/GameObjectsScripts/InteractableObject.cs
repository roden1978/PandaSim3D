using UnityEngine;

namespace GameObjectsScripts
{
    public abstract class InteractableObject: MonoBehaviour
    {
       public int Value { get; set; }
       public bool Hide { get; protected set; } = true;
    }
}