using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] private Button _continue;
        [SerializeField] private Button _exit;
        private const string sceneName = "MainMenu";
        private void Awake()
        {
            _continue.onClick.AddListener(OnContinue);
            _exit.onClick.AddListener(OnExit);
        }

        private void OnExit()
        {
            LoadMainMenu();
        }

        private static void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync(sceneName);
        }

        private void OnContinue()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }
}
