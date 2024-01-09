using System;
using Cysharp.Threading.Tasks;
using Infrastructure.GameStates;

namespace Infrastructure
{
    public interface ISceneLoader
    {
        UniTask<bool> LoadScene(string sceneName);
        UniTask<bool> UnLoadScene(string sceneName);
    }
}