using System;
using System.Collections.Generic;
using Common;
using Infrastructure.AssetManagement;
using Infrastructure.GameStates;
using PlayerScripts;
using UnityEngine;

namespace Infrastructure
{
    public class Game
    {
        private readonly LoadGameSettings _loadGameSettings;
        private readonly LoadProgress _loadProgress;
        private readonly SceneLoader _sceneLoader;
        private bool _pause;
        private readonly Dictionary<Type, IUpdateable> _updateableModels = new();
        private PlayerProgress _playerProgress;

        public Game(LoadGameSettings loadGameSettings, LoadProgress loadProgress, SceneLoader sceneLoader)
        {
            _loadGameSettings = loadGameSettings;
            _loadProgress = loadProgress;
            _sceneLoader = sceneLoader;
        }

        public void Start()
        {
            Debug.Log("Game Start");
            //_progressWriters.Add(_walletService);
            //LoadSettings();
            //LoadProgress();
            LoadScene(AssetPaths.CurtainSceneName);
        }

        public void Restart()
        {
        }

        private void LoadSettings()
        {
            Debug.Log("Load settings");
            _loadGameSettings.LoadSettings();
        }

        private async void LoadProgress()
        {
            Debug.Log("Load progress");
            _playerProgress = await _loadProgress.LoadPlayerProgress();
        }

        private async void LoadScene(string sceneName)
        {
            Debug.Log("Load scene");
            await _sceneLoader.LoadScene(sceneName);
        }

        public void Register<T>(T model) where T : IUpdateable
        {
            Type type = model.GetType();
            _updateableModels.Add(type, model);
        }

        public void UnRegister<T>(T model) where T : IUpdateable
        {
            Type type = model.GetType();
            if (_updateableModels.ContainsKey(type))
            {
                _updateableModels.Remove(type);
            }
        }

        public void ClearUpdateable()
        {
            _updateableModels.Clear();
        }

        public void Pause()
        {
            _pause = true;
        }

        public void Resume()
        {
            _pause = false;
        }

        public void Tick()
        {
            Debug.Log("Game update");
            if (_pause) return;

            foreach (var updateable in _updateableModels)
            {
                updateable.Value.OnUpdate();
            }
        }
    }
}