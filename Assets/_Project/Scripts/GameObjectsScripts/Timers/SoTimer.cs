﻿using UnityEngine;

namespace GameObjectsScripts.Timers
{
    [CreateAssetMenu(fileName = "New Timer Data", menuName = "StaticData/TimerData")]
    public class SoTimer : ScriptableObject
    {
        [SerializeField] private TimerType _type;
        [SerializeField] private float _duration;
        [SerializeField] private GameObject _timerPrefab = null!;
        [SerializeField] private Color _timerColor = Color.white;
        [SerializeField] [Range(0, .2f)] private float _moodDecrease;
        [SerializeField] private bool _canStart;

        public TimerType Type => _type;
        public float Duration => _duration;
        public GameObject TimerPrefab => _timerPrefab;
        public Color TimeColor => _timerColor;
        public float MoodDecrease => _moodDecrease;
        public bool CanStart => _canStart;
    }
}