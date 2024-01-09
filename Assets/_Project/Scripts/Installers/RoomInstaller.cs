using System;
using Infrastructure.AssetManagement;
using Infrastructure.PickableObjectSpawners;
using PlayerScripts;
using Services.SaveLoad.PlayerProgress;
using Services.StaticData;
using StaticData;
using UI;
using UnityEngine;
using Zenject;

public class RoomInstaller : MonoInstaller
{
    private PrefabsStorage _prefabsStorage;
    private ISaveLoadStorage _saveLoadStorage;
    private IStaticDataService _staticDataService;
    private LevelStaticData _levelStaticData;
    private IPersistentProgress _persistenceProgress;

    [Inject]
    public void Construct(PrefabsStorage prefabsStorage, ISaveLoadStorage saveLoadStorage,
        IStaticDataService staticDataService, IPersistentProgress persistentProgress)
    {
        _prefabsStorage = prefabsStorage;
        _saveLoadStorage = saveLoadStorage;
        _staticDataService = staticDataService;
        _persistenceProgress = persistentProgress;
        
        _levelStaticData = _staticDataService.GetLevelStaticData(AssetPaths.SceneName);
    }

    public override void InstallBindings()
    {
        BindGuiHolder();
        BindDialogManager();
        BindInventory();
        BindInventoryDialog();
        BindShop();
        BindHud();
        BindPlate();
        BindEgg();
        BindPlayer();
    }

    private void BindShop()
    {
        GameObject shopDialog = _prefabsStorage.Get(typeof(ShopDialog));
        Container.Bind<ShopDialog>()
            .FromComponentInNewPrefab(shopDialog)
            .UnderTransformGroup("GuiHolder")
            .AsSingle()
            .NonLazy();
    }

    private void BindInventory()
    {
        Container.BindInterfacesAndSelfTo<Inventory>().AsSingle();
        Inventory inventory = Container.Resolve<Inventory>();
        _saveLoadStorage.RegisterInSaveLoadRepositories(inventory);
    }

    private void BindInventoryDialog()
    {
        GameObject inventoryDialog = _prefabsStorage.Get(typeof(InventoryDialog));
        Container.Bind<InventoryDialog>()
            .FromComponentInNewPrefab(inventoryDialog)
            .UnderTransformGroup("GuiHolder")
            .AsSingle()
            .NonLazy();
    }

    private void BindDialogManager()
    {
        Container.Bind<DialogManager>().AsSingle();
    }

    private void BindGuiHolder()
    {
        Debug.Log($"Instantiate GuiHolder start");
        GameObject guiHolder = _prefabsStorage.Get(typeof(GuiHolder));
        Container
            .Bind<GuiHolder>()
            .FromComponentInNewPrefab(guiHolder)
            .WithGameObjectName("GuiHolder")
            .AsSingle()
            .NonLazy();
        Debug.Log($"Instantiate GuiHolder end");
    }

    private void BindHud()
    {
        Debug.Log($"Instantiate hud start ");
        GameObject hud = _prefabsStorage.Get(Type.GetType("Hud")); //almost typeof(Hud)
        Container
            .Bind<Hud>()
            .FromComponentInNewPrefab(hud)
            .WithGameObjectName(nameof(Hud))
            .UnderTransformGroup("GuiHolder")
            .AsSingle()
            .NonLazy();
        Debug.Log($"Instantiate hud end {hud}");
    }

    private void BindPlate()
    {
        EnvironmentObjectSpawnData plateSpawnData = _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.Plate);
        GameObject plateSpawner = Container.InstantiatePrefab(_prefabsStorage.Get(typeof(PlateSpawner)),
            plateSpawnData.Position, Quaternion.identity, null);
        _saveLoadStorage.RegisterInSaveLoadRepositories(plateSpawner);
    }

    private void BindEgg()
    {
        if(_persistenceProgress.PlayerProgress.PlayerState.FirstStartGame)
        {
            EnvironmentObjectSpawnData eggSpawnData =
                _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.Egg);
            GameObject egg = Container.InstantiatePrefab(_prefabsStorage.Get(typeof(Egg)),
                eggSpawnData.Position, Quaternion.identity, null);
            _saveLoadStorage.RegisterInSaveLoadRepositories(egg);
        }
    }

    private void BindPlayer()
    {
        LevelStaticData levelStaticData = _staticDataService.GetLevelStaticData(AssetPaths.SceneName);
        Vector3 position = levelStaticData.PlayerSpawnPoint;
        GameObject playerPrefab = _prefabsStorage.Get(typeof(Player));
        IPositionAdapter positionAdapter = playerPrefab.GetComponent<IPositionAdapter>();
        positionAdapter.Position = position;
        
        Container.Bind<Player>()
            .FromComponentInNewPrefab(playerPrefab)
            .WithGameObjectName(nameof(Player))
            .AsSingle()
            .NonLazy();
    }
}