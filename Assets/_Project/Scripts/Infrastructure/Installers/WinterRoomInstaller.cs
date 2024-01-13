using System;
using GameObjectsScripts;
using Infrastructure.AssetManagement;
using PlayerScripts;
using Services.SaveLoad.PlayerProgress;
using Services.StaticData;
using StaticData;
using UI;
using UnityEngine;
using Zenject;

public class WinterRoomInstaller : MonoInstaller
{
    private PrefabsStorage _prefabsStorage;
    private ISaveLoadStorage _saveLoadStorage;
    private IStaticDataService _staticDataService;
    private LevelStaticData _levelStaticData;
    private IPersistentProgress _persistenceProgress;

    private Transform _guiHolderTransform;

    [Inject]
    public void Construct(PrefabsStorage prefabsStorage, ISaveLoadStorage saveLoadStorage,
        IStaticDataService staticDataService, IPersistentProgress persistentProgress)
    {
        _prefabsStorage = prefabsStorage;
        _saveLoadStorage = saveLoadStorage;
        _staticDataService = staticDataService;
        _persistenceProgress = persistentProgress;

        _levelStaticData = _staticDataService.GetLevelStaticData(AssetPaths.WinterRoomSceneName);
        _saveLoadStorage.ClearGameObjectsType();
    }

    public override void InstallBindings()
    {
        BindGuiHolder();
        BindDialogManager();
        BindHud();
        //BindInputNameDialog();
        //BindEgg();
        BindPlayer();
        BindCrateAkaBindPlate();
        BindInventory();
        BindInventoryDialog();
        BindShop();
        BindSnowman();
    }

    private void BindShop()
    {
        GameObject shopDialog = _prefabsStorage.Get(typeof(ShopDialog));
        Container.InstantiatePrefab(shopDialog, _guiHolderTransform);
    }

    private void BindInputNameDialog()
    {
        GameObject prefab = _prefabsStorage.Get(typeof(InputNameDialog));

        if (false == _persistenceProgress.PlayerProgress.PlayerState.FirstStartGame)
        {
            _prefabsStorage.Unregister(prefab);
            return;
        }

        GameObject inventoryNameDialog = Container.InstantiatePrefab(prefab, _guiHolderTransform);
        Container.Bind<InputNameDialog>().FromComponentOn(inventoryNameDialog).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(inventoryNameDialog);
    }

    private void BindInventory()
    {
        Container.BindInterfacesAndSelfTo<Inventory>().AsSingle();
        Inventory inventory = Container.Resolve<Inventory>();
        _saveLoadStorage.RegisterInSaveLoadRepositories(inventory);
    }

    private void BindInventoryDialog()
    {
        GameObject prefab = _prefabsStorage.Get(typeof(InventoryDialog));
        GameObject inventoryDialog = Container.InstantiatePrefab(prefab, _guiHolderTransform);
        Container.Bind<InventoryDialog>().FromComponentOn(inventoryDialog).AsSingle();
    }

    private void BindDialogManager()
    {
        Container.Bind<DialogManager>().AsSingle();
    }

    private void BindGuiHolder()
    {
        Debug.Log($"Instantiate GuiHolder start");
        GameObject prefab = _prefabsStorage.Get(typeof(GuiHolder));
        GameObject guiHolder = Container.InstantiatePrefab(prefab);
        _guiHolderTransform = guiHolder.transform;

        Container.Bind<GuiHolder>().FromComponentOn(guiHolder).AsSingle();
    }

    private void BindHud()
    {
        Debug.Log($"Instantiate hud start ");
        GameObject prefab = _prefabsStorage.Get(Type.GetType("Hud")); //almost typeof(Hud)
        GameObject hud = Container.InstantiatePrefab(prefab, _guiHolderTransform);
        Container.Bind<Hud>().FromComponentOn(hud).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(hud);
    }

    private void BindCrateAkaBindPlate()
    {
        EnvironmentObjectSpawnData crateData =
            _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.Crate);
        GameObject prefab = _prefabsStorage.Get(typeof(Crate));
        IPositionAdapter positionAdapter = prefab.GetComponent<IPositionAdapter>();
        positionAdapter.Position = crateData.Position;

        Container.Bind<Plate>()
            .FromComponentInNewPrefab(prefab)
            .WithGameObjectName("Crate")
            .AsSingle()
            .NonLazy();
    }

    private void BindEgg()
    {
        GameObject eggPrefab = _prefabsStorage.Get(typeof(Egg));
        
        if (false == _persistenceProgress.PlayerProgress.PlayerState.FirstStartGame)
        {
            _prefabsStorage.Unregister(eggPrefab);
            return;
        }

        EnvironmentObjectSpawnData eggSpawnData =
            _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.Egg);
        GameObject egg = Container.InstantiatePrefab(eggPrefab, eggSpawnData.Position, Quaternion.identity, null);
        _saveLoadStorage.RegisterInSaveLoadRepositories(egg);
    }

    private void BindPlayer()
    {
        Vector3 position = _levelStaticData.PlayerSpawnPoint;
        Quaternion rotation = _levelStaticData.PlayerRotation;
        GameObject playerPrefab = _prefabsStorage.Get(typeof(Player));
        IPositionAdapter positionAdapter = playerPrefab.GetComponent<IPositionAdapter>();
        IRotationAdapter rotationAdapter = playerPrefab.GetComponent<IRotationAdapter>();
        positionAdapter.Position = position;
        rotationAdapter.Rotation = rotation;

        Container.Bind<Player>()
            .FromComponentInNewPrefab(playerPrefab)
            .WithGameObjectName(nameof(Player))
            .AsSingle()
            .NonLazy();
    }
    
    private void BindSnowman()
    {
        Debug.Log($"Instantiate snowman start ");
        GameObject prefab = _prefabsStorage.Get(typeof(Snowman)); 
        EnvironmentObjectSpawnData snowmanSpawnData =
            _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.Snowman);
        GameObject snowman = Container.InstantiatePrefab(prefab, snowmanSpawnData.Position, Quaternion.identity, null);
        _saveLoadStorage.RegisterInSaveLoadRepositories(snowman);
    }
}