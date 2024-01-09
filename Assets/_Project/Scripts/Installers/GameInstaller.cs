using Infrastructure;
using Infrastructure.GameStates;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<LoadGameSettings>().AsSingle();
        Container.Bind<LoadProgress>().AsSingle();
        Container.Bind<Game>().AsSingle().NonLazy();
    }
}