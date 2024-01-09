using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        //[SerializeField] private Canvas _canvas;

        private void Start()
        {
            //Destroy(_canvas.gameObject);
            DontDestroyOnLoad(this);
            LoadScene(AssetPaths.CurtainSceneName);
        }

        private async void LoadScene(string sceneName)
        {
            Debug.Log("Load scene");
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}