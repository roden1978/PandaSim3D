using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;

namespace Infrastructure.GameStates
{
    public class LoadProgress
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IPersistentProgress _persistentProgress;

        public LoadProgress(ISaveLoadService saveLoadService, IPersistentProgress persistentProgress)
        {
            _saveLoadService = saveLoadService;
            _persistentProgress = persistentProgress;
        }

        public async UniTask<PlayerProgress> LoadPlayerProgress()
        {
            _persistentProgress.PlayerProgress = await _saveLoadService.LoadProgress() ?? CreatePlayerProgress();

            return _persistentProgress.PlayerProgress;
        }
        private PlayerProgress CreatePlayerProgress()
        {
            PlayerProgress playerProgress = new();

            playerProgress.PlayerState.SceneName = AssetPaths.SceneName;
            playerProgress.PlayerState.CurrentHealth = playerProgress.StaticPlayerData.Health;
            playerProgress.PlayerState.MaxHealth = playerProgress.StaticPlayerData.MaxHealth;
            playerProgress.PlayerState.Dream = playerProgress.StaticPlayerData.Dream;
            playerProgress.PlayerState.Meal = playerProgress.StaticPlayerData.Meal;
            playerProgress.PlayerState.Mood = playerProgress.StaticPlayerData.Mood;
            playerProgress.PlayerState.Toilet = playerProgress.StaticPlayerData.Toilet;
           
            return playerProgress;
        }
    }
}