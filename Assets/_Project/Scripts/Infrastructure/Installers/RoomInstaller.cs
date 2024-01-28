using Infrastructure.AssetManagement;
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

    private Transform _guiHolderTransform;
    private Transform _hudTransform;

    [Inject]
    public void Construct(PrefabsStorage prefabsStorage, ISaveLoadStorage saveLoadStorage,
        IStaticDataService staticDataService, IPersistentProgress persistentProgress)
    {
        _prefabsStorage = prefabsStorage;
        _saveLoadStorage = saveLoadStorage;
        _staticDataService = staticDataService;
        _persistenceProgress = persistentProgress;

        _levelStaticData = _staticDataService.GetLevelStaticData(AssetPaths.RoomSceneName);
        _saveLoadStorage.ClearGameObjectsType();
    }

    public override void InstallBindings()
    {
        BindGuiHolder();
        BindDialogManager();
        BindHud();
        BindTimersPrincipal();
        BindInputNameDialog();
        BindEgg();
        BindPlayer();
        BindInventory();
        BindPlate();
        BindInventoryDialog();
        BindShop();
        BindPoop();
        BindTray();
        BindTrayView();
    }

    private void BindGuiHolder()
    {
        Debug.Log($"Instantiate GuiHolder start");
        GameObject prefab = _prefabsStorage.Get(typeof(GuiHolder));
        GameObject guiHolder = Container.InstantiatePrefab(prefab);
        _guiHolderTransform = guiHolder.transform;

        Container.Bind<GuiHolder>().FromComponentOn(guiHolder).AsSingle();
    }
    private void BindPoop()
    {
        EnvironmentObjectSpawnData poopData =
            _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.Poop);
        GameObject prefab = _prefabsStorage.Get(typeof(Poop));
        IPositionAdapter positionAdapter = prefab.GetComponentInChildren<IPositionAdapter>(true);
        positionAdapter.Position = poopData.Position;
        GameObject poop = Container.InstantiatePrefab(prefab);
        poop.gameObject.name = nameof(Poop);
        Container.BindInterfacesAndSelfTo<Poop>().FromComponentOn(poop).AsSingle();
        //_saveLoadStorage.RegisterInSaveLoadRepositories(poop);
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

   

    private void BindHud()
    {
        Debug.Log($"Instantiate Hud start ");
        GameObject prefab = _prefabsStorage.Get(typeof(Hud));
        GameObject hud = Container.InstantiatePrefab(prefab, _guiHolderTransform);
        _hudTransform = hud.transform;
        Container.Bind<Hud>().FromComponentOn(hud).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(hud);
    }
    
    private void BindTimersPrincipal()
    {
        Debug.Log($"Instantiate TimersPrincipal start ");
        GameObject prefab = _prefabsStorage.Get(typeof(TimersPrincipal));
        GameObject timersPrincipal = Container.InstantiatePrefab(prefab, _hudTransform);
        Container.BindInterfacesAndSelfTo<TimersPrincipal>().FromComponentOn(timersPrincipal).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(timersPrincipal);
    }

    private void BindPlate()
    {
        EnvironmentObjectSpawnData plateData =
            _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.Plate);
        GameObject prefab = _prefabsStorage.Get(typeof(Plate));
        IPositionAdapter positionAdapter = prefab.GetComponentInChildren<IPositionAdapter>(true);
        positionAdapter.Position = plateData.Position;
        GameObject plate = Container.InstantiatePrefab(prefab);
        plate.gameObject.name = nameof(Plate);
        Container.BindInterfacesAndSelfTo<Plate>().FromComponentOn(plate).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(plate);
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
        GameObject playerPrefab = _prefabsStorage.Get(typeof(Player));
        Vector3 position = _levelStaticData.PlayerSpawnPoint;
        Quaternion rotation = _levelStaticData.PlayerRotation;
        playerPrefab.SetPositionAdapterValue(position, rotation);

        Container.Bind<Player>()
            .FromComponentInNewPrefab(playerPrefab)
            .WithGameObjectName(nameof(Player))
            .AsSingle()
            .NonLazy();
    }
    
    private void BindTray()
    {
        Container.BindInterfacesAndSelfTo<Tray>().AsSingle();
        Tray tray = Container.Resolve<Tray>();
        _saveLoadStorage.RegisterInSaveLoadRepositories(tray);
    }
    private void BindTrayView()
    {
        EnvironmentObjectSpawnData trayData =
            _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.TrayView);
        GameObject prefab = _prefabsStorage.Get(typeof(TrayView));
        GameObject tray = Container.InstantiatePrefab(prefab, trayData.Position, Quaternion.identity, null);
        Container.BindInterfacesAndSelfTo<TrayView>().FromComponentOn(tray).AsSingle();
        //_saveLoadStorage.RegisterInSaveLoadRepositories(tray);
    }
}