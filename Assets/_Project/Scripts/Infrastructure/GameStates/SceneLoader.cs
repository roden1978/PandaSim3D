using Cysharp.Threading.Tasks;
using Infrastructure.Factories;

namespace Infrastructure.GameStates
{
    public class SceneLoader
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IGameFactory _gameFactory;

        public SceneLoader(ISceneLoader sceneLoader, IGameFactory gameFactory)
        {
            _sceneLoader = sceneLoader;
            _gameFactory = gameFactory;
        }
        
        public async void LoadScene(string sceneName)
        {
            _gameFactory.CleanUp();
            
            await _sceneLoader.LoadScene(sceneName);
            //while (result.Status != UniTaskStatus.Succeeded)
            //    await UniTask.Yield();
        }
    }
}