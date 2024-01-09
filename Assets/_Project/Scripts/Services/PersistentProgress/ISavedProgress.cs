using Data;

namespace Services.PersistentProgress
{
    public interface ISavedProgress : ISavedProgressReader
    {
        public void UpdateProgress(PlayerProgress playerProgress);
    }
}