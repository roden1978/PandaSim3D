using Infrastructure.AssetManagement;
using Zenject;

public class ProgressLoadingInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindAssetLoader();
    }

    private void BindAssetLoader()
    {
        Container.BindInterfacesAndSelfTo<ProgressLoader>().AsSingle();
    }
}