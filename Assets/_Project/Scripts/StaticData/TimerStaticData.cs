using UnityEngine;
using UnityEngine.UI;

namespace StaticData
{
    [CreateAssetMenu(fileName = "New Timer Data", menuName = "StaticData/TimerData")]
    public class TimerStaticData : ScriptableObject
    {
        [SerializeField] private TimerType _type;
        [SerializeField] private int _duration;

        public TimerType Type => _type;
        public int Duration => _duration;
    }
    
    public enum TimerType
    {
        None = 0,
        Mood = 1,
        Meal = 2,
        Sleep = 3,
        Poop = 4,
        Cold = 5,
    }
}