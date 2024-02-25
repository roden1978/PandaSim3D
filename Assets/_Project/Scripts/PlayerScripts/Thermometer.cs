using System;
using GameObjectsScripts.Timers;
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
        private string _roomName;
        private ISaveLoadStorage _saveLoadStorage;
        private ISceneLoader _sceneLoader;
        private Timer _timer;
        private TimersPrincipal _timersPrincipal;

        [Inject]
        public void Construct(TimersPrincipal timersPrincipal, ISaveLoadStorage saveLoadStorage,
            ISceneLoader sceneLoader)
        {
            _saveLoadStorage = saveLoadStorage;
            _sceneLoader = sceneLoader;
            _timersPrincipal = timersPrincipal;
            _timer = timersPrincipal.GetTimerByType(TimerType.Thermo);
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
            _timer ??= _timersPrincipal.GetTimerByType(TimerType.Thermo);
            RoomsType type = Enum.Parse<RoomsType>(_roomName);
            switch (type)
            {
                case AssetPaths.RoomSceneName:
                    if (value)
                        RestartTimer();
                    else
                        StopTimer();

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
            _timer.Restart();
        }

        private void StopTimer()
        {
            _timer.Stop();
            _timer.Reset();
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
            if (playerProgress.PlayerState.FirstStartGame) return;

            UpdateThermometerState(_hatHolder.ItemType != ItemType.None);
        }

        public void SaveProgress(PlayerProgress playerProgress)
        {
        }
    }
}