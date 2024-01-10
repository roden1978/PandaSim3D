using Infrastructure;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Died : MonoBehaviour
    {
        [SerializeField] private Button _continue;
        [SerializeField] private Button _exit;
        private PlayerProgress _playerProgress;
        //private IGamesStateMachine _stateMachine;
        private const string MainMenuSceneName = "MainMenu";

        
        private void Awake()
        {
            _continue.onClick.AddListener(OnContinue);
            _exit.onClick.AddListener(OnExit);
            //_playerProgress = ServiceLocator.Container.Single<IPersistentProgressService>().PlayerProgress;
            //_stateMachine = ServiceLocator.Container.Single<IGamesStateMachine>();
        }

        private void OnExit()
        {
            LoadMainMenu();
        }

        private static void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync(MainMenuSceneName);
        }

        private void OnContinue()
        {
            Time.timeScale = 1f;
            //string sceneName = _playerProgress.WorldData.PositionOnLevel.SceneName;
            //_stateMachine.Enter<LoadLevelState, string>(sceneName);
        }
    }
}
