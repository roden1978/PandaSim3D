using Data;
using Infrastructure.Services;

namespace Services.PersistentProgress
{
    public interface IPersistentProgressService : IService
    {
        public PlayerProgress PlayerProgress { get; set; }
        public Settings Settings { get; set; }
    }

    public class PersistentProgressService : IPersistentProgressService
    {
        public PlayerProgress PlayerProgress { get; set; }
        public Settings Settings { get; set; }
    }
}