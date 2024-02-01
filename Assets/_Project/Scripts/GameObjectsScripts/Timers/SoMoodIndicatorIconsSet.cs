using System.Collections.Generic;
using UnityEngine;

namespace GameObjectsScripts.Timers
{
    [CreateAssetMenu(fileName = "New Mood Indicator Icon Set Data", menuName = "StaticData/MoodIndicatorIconSetData")]
    public class SoMoodIndicatorIconsSet : ScriptableObject
    {
        [SerializeField] private List<Sprite> _soSprites;

        public IReadOnlyCollection<Sprite> SoSprites => _soSprites;
    }
}