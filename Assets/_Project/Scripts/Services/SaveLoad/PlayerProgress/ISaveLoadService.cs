using Cysharp.Threading.Tasks;

public interface ISaveLoadService
{
    void SaveProgress();
    UniTask<PlayerProgress> LoadProgress();
    public void SaveSettings();
    UniTask<Settings> LoadSettings();
}