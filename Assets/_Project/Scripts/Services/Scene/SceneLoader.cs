using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class SceneLoader : ISceneLoader
    {
        public async UniTask<bool> LoadScene(string sceneName)
        {
            AsyncOperation wait = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            
            while (!wait.isDone)
             await UniTask.Yield();

            Scene newScene = SceneManager.GetSceneByName(sceneName);
            while (!newScene.isLoaded)
                await UniTask.Yield();
            
            SceneManager.SetActiveScene(newScene);
            
            return true;
        }
        public async UniTask<bool> UnLoadScene(string sceneName)
        {
            AsyncOperation wait = SceneManager.UnloadSceneAsync(sceneName);
            
            while (!wait.isDone)
                await UniTask.Yield();
            
            return true;
        }

        public static string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}
