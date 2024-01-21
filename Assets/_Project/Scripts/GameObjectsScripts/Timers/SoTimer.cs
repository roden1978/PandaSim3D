using UnityEngine;

namespace GameObjectsScripts.Timers
{
    [CreateAssetMenu(fileName = "New Timer Data", menuName = "StaticData/TimerData")]
    public class SoTimer : ScriptableObject
    {
        [SerializeField] private TimerType _type;
        [SerializeField] private int _duration;
        [SerializeField] private GameObject _timerPrefab;
        [SerializeField] private Color _timerColor = Color.white;

        public TimerType Type => _type;
        public int Duration => _duration;
        public GameObject TimerPrefab => _timerPrefab;

        public Color TimeColor => _timerColor;
    }
}