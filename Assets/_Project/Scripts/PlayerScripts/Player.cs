using System;
using UnityEngine;

namespace PlayerScripts
{
    public class Player : MonoBehaviour, ISavedProgress, IHealth, IPositionAdapter, IRotationAdapter
    {
        public Action<int> Death;
        public int HP { get; private set; }
        public int MaxHealth { get; private set; }
        private int _currentLivesAmount;

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0)
            {
                Death?.Invoke(_currentLivesAmount -= 1);
            }
        }

        public void Heal(int health)
        {
            if (HP < MaxHealth)
            {
                HP += health;
            }
        }

        public void TakeBonusLive(int delta)
        {
            _currentLivesAmount += delta;
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
            /*HP = playerProgress.PlayerState.CurrentHealth;
            MaxHealth = playerProgress.PlayerState.MaxHealth;*/
        }

        public void SaveProgress(PlayerProgress playerProgress)
        {
            //playerProgress.PlayerState.CurrentHealth = HP;
        }
    }
}