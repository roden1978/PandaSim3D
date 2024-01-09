using Data;

namespace Services.PersistentProgress
{
    public interface ISavedProgressReader
    {
        void LoadProgress(PlayerProgress playerProgress);
    }
}