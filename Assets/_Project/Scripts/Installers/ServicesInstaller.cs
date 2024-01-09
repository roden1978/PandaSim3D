using Infrastructure;
using Infrastructure.AssetManagement;
using Services.Pool;
using Services.SaveLoad.PlayerProgress;
using Services.StaticData;
using Zenject;

public class ServicesInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindAssetProvider();
        BindAssetStorage();
        BindStaticDataService();
        BindPersistentProgress();
        BindSaveLoadStorage();
        BindSaveLoadService();
        BindSceneLoader();
        BindPoolService();
        BindEventBus();
        BindWallet();
        //BindAssetLoader();
    }

    private void BindAssetProvider()
    {
        Container.BindInterfacesAndSelfTo<AssetProvider>().AsSingle();
    }

    private void BindAssetStorage()
    {
        Container.BindInterfacesAndSelfTo<PrefabsStorage>().AsSingle().NonLazy();
    }

    private void BindStaticDataService()
    {
        Container.BindInterfacesAndSelfTo<StaticDataService>().AsSingle();
    }

    private void BindPersistentProgress()
    {
        Container.BindInterfacesAndSelfTo<PersistentProgress>().AsSingle();
    }

    private void BindSaveLoadStorage()
    {
        Container.BindInterfacesAndSelfTo<SaveLoadStorage>().AsSingle();
    }

    private void BindSaveLoadService()
    {
        Container.BindInterfacesAndSelfTo<SaveLoadService>().AsSingle().NonLazy();
    }

    private void BindSceneLoader()
    {
        Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
    }

    private void BindPoolService()
    {
        Container.BindInterfacesAndSelfTo<PoolService>().AsSingle();
    }

    private void BindEventBus()
    {
        Container.BindInterfacesAndSelfTo<EventBus>().AsSingle();
    }

    private void BindWallet()
    {
        Container.BindInterfacesAndSelfTo<WalletService>().AsSingle();
    }
}