using System.Collections.Generic;
using UnityEngine;

namespace GameObjectsScripts.Timers
{
    [CreateAssetMenu(fileName = "New Timers Set Data", menuName = "StaticData/TimersSetData")]
    public class SoTimersSet : ScriptableObject
    {
        [SerializeField] private List<SoTimer> _soTimers;

        public IReadOnlyCollection<SoTimer> SoTimers => _soTimers;
    }
}