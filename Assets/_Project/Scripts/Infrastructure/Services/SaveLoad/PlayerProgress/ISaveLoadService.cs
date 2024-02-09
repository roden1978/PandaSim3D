using Cysharp.Threading.Tasks;

public interface ISaveLoadService
{
    void SaveProgress();
    PlayerProgress LoadProgress();
    public void SaveSettings();
    Settings LoadSettings();
}