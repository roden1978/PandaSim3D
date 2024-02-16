using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameObjectsScripts.Timers
{
    [CreateAssetMenu(fileName = "New Timers Set Data", menuName = "StaticData/TimersSetData")]
    public class SoTimersSet : ScriptableObject
    {
        [SerializeField] private List<SoTimer> _soCommonTimers;
        [SerializeField] private List<RoomTimers> _soRoomTimers;

        public IReadOnlyCollection<SoTimer> SoCommonTimers => _soCommonTimers;
        public IReadOnlyCollection<RoomTimers> SoRoomTimers => _soRoomTimers;
    }

    [Serializable]
    public sealed class RoomTimers
    {
        public RoomsType RoomType;
        public List<SoTimer> SoTimers;
    }
}