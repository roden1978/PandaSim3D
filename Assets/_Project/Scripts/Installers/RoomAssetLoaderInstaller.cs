using Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;

public class RoomAssetLoaderInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindAssetLoader();
    }

    private void BindAssetLoader()
    {
        Debug.Log("Bind asset loader");
        Container.BindInterfacesAndSelfTo<AssetLoader>().AsSingle();
    }
}