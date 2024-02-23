public interface ISaveLoadService
{
    void SaveProgress();
    PlayerProgress LoadProgress();
    void SaveSettings();
    Settings LoadSettings();
    void Delete();
}