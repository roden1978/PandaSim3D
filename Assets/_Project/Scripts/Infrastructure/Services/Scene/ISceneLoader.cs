using Cysharp.Threading.Tasks;

namespace Infrastructure
{
    public interface ISceneLoader
    {
        UniTask<bool> LoadScene(string sceneName);
        UniTask<bool> UnLoadScene(string sceneName);
    }
}