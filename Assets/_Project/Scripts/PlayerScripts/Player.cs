using System;
using System.Collections.Generic;
using Infrastructure.AssetManagement;
using Infrastructure.Services.EventBus.Signals.PlayerSignals;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;

namespace PlayerScripts
{
    public class Player : MonoBehaviour, IPositionAdapter, IRotationAdapter, IPointerClickHandler, ISavedProgress
    {
       private State _state;
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

        [SerializeField] private List<Collider> _awakeColliders;
        [SerializeField] private List<Collider> _sleepColliders;
        private IEventBus _eventBus;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void SetActiveColliders(bool value)
        {
                _awakeColliders.ForEach(x => x.isTrigger = !value);
                _sleepColliders.ForEach(x => x.isTrigger = value);
        }

        public void SetState(State state)
        {
            _state = state;
            _eventBus.Invoke(new ChangePlayerStateSignal(state));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ChangePlayerState?.Invoke(State.Awake);
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
            _state = playerProgress.PlayerState.State;
            SetActiveColliders(true);
            string currentSceneName = SceneManager.GetActiveScene().name;
            
            if(currentSceneName == AssetPaths.RoomSceneName.ToString())
            {
                Position = playerProgress.PlayerState.Position.Vector3DataToVector3();
                Rotation = playerProgress.PlayerState.Rotation.QuaternionDataToQuaternion();
            }
        }

        public void SaveProgress(PlayerProgress playerProgress)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName == AssetPaths.RoomSceneName.ToString())
            {
                playerProgress.PlayerState.State = _state;
                playerProgress.PlayerState.Position = transform.position.Vector3ToVector3Data();
                playerProgress.PlayerState.Rotation = transform.rotation.QuaternionToQuaternionData();
            }
        }

        public event Action<State> ChangePlayerState;
    }

    public enum State
    {
        Sleep = 0,
        Awake = 1,
    }
}