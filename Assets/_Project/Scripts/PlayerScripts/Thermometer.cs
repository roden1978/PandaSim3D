using System;
using Infrastructure;
using Infrastructure.AssetManagement;
using Services.SaveLoad.PlayerProgress;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PlayerScripts
{
    public class Thermometer : MonoBehaviour, ISavedProgress
    {
        [SerializeField] private HatHolder _hatHolder;
        private TimersPrincipal _timerPrincipal;
        private string _roomName;
        private ISaveLoadStorage _saveLoadStorage;
        private ISceneLoader _sceneLoader;

        [Inject]
        public void Construct(TimersPrincipal timersPrincipal, ISaveLoadStorage saveLoadStorage, ISceneLoader sceneLoader)
        {
            _timerPrincipal = timersPrincipal;
            _saveLoadStorage = saveLoadStorage;
            _sceneLoader = sceneLoader;
            _hatHolder.HasHat += OnHasHat;
            _saveLoadStorage.RegisterInSaveLoadRepositories(gameObject);
        }

        private void Start()
        {
            _roomName = SceneManager.GetActiveScene().name;
        }

        private void OnHasHat(bool value)
        {
            UpdateThermometerState(value);
        }

        private void UpdateThermometerState(bool value)
        {
            RoomsType type = Enum.Parse<RoomsType>(_roomName);
            switch (type)
            {
                case AssetPaths.RoomSceneName:
                    if (value)
                        RestartTimer();
                    else
                    {
                        StopTimer();
                    }

                    break;
                case AssetPaths.WinterRoomSceneName:
                    if (value)
                    {
                        StopTimer();
                        //BackToRoom();
                    }
                    else
                        RestartTimer();

                    break;
                case RoomsType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RestartTimer()
        {
            _timerPrincipal.ReStartTimerByType(TimerType.Thermo);
        }

        private void StopTimer()
        {
            _timerPrincipal.StopTimerByType(TimerType.Thermo);
            _timerPrincipal.ResetTimerByType(TimerType.Thermo);
        }

        private void BackToRoom()
        {
            _sceneLoader.LoadScene(AssetPaths.RoomSceneName.ToString());
        }

        private void OnDisable()
        {
            _hatHolder.HasHat -= OnHasHat;
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
            if (false == playerProgress.PlayerState.FirstStartGame)
                UpdateThermometerState(_hatHolder.ItemType != ItemType.None);
        }

        public void SaveProgress(PlayerProgress playerProgress)
        {
        }
    }
}