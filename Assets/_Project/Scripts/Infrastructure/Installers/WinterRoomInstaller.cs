using System;
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
    private Transform _hudTransform;
    private IWalletService _wallet;

    [Inject]
    public void Construct(PrefabsStorage prefabsStorage, ISaveLoadStorage saveLoadStorage,
        IStaticDataService staticDataService, IPersistentProgress persistentProgress, IWalletService wallet)
    {
        _prefabsStorage = prefabsStorage;
        _saveLoadStorage = saveLoadStorage;
        _staticDataService = staticDataService;
        _persistenceProgress = persistentProgress;
        _wallet = wallet;

        _levelStaticData = _staticDataService.GetLevelStaticData(AssetPaths.WinterRoomSceneName.ToString());
        _saveLoadStorage.ClearAll();
    }

    public override void InstallBindings()
    {
        RegisterWallet();
        BindGuiHolder();
        BindDialogManager();
        BindHud();
        BindTimersPrincipal();
        BindInventory();
        BindPlayer();
        BindBackpackDrawer();
        BindBackpackInventoryDialog();
        BindShop();
        BindSnowman();
        BindTray();
        BindMenuDialog();
        BindGameOverDialog();
    }
    
    private void RegisterWallet()
    {
        _saveLoadStorage.RegisterInSaveLoadRepositories(_wallet);
    }

    private void BindMenuDialog()
    {
        GameObject prefab = _prefabsStorage.Get(typeof(MenuDialog));
        GameObject menuDialog = Container.InstantiatePrefab(prefab, _guiHolderTransform);
        Container.BindInterfacesAndSelfTo<MenuDialog>().FromComponentOn(menuDialog).AsSingle();
    }
    
    private void BindGameOverDialog()
    {
        GameObject prefab = _prefabsStorage.Get(typeof(GameOverDialog));
        GameObject gameOverDialog = Container.InstantiatePrefab(prefab, _guiHolderTransform);
        Container.BindInterfacesAndSelfTo<GameOverDialog>().FromComponentOn(gameOverDialog).AsSingle();
    }
    private void BindShop()
    {
        GameObject shopDialog = _prefabsStorage.Get(typeof(ShopDialog));
        Container.InstantiatePrefab(shopDialog, _guiHolderTransform);
    }

    private void BindInventory()
    {
        Container.BindInterfacesAndSelfTo<Inventory>().AsSingle();
        Inventory inventory = Container.Resolve<Inventory>();
        _saveLoadStorage.RegisterInSaveLoadRepositories(inventory);
    }

    private void BindBackpackInventoryDialog()
    {
        GameObject prefab = _prefabsStorage.Get(typeof(BackpackInventoryDialog));
        GameObject backPackInventoryDialog = Container.InstantiatePrefab(prefab, _guiHolderTransform);
        Container.Bind<BackpackInventoryDialog>().FromComponentOn(backPackInventoryDialog).AsSingle();
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
        _hudTransform = hud.transform;
        Container.Bind<Hud>().FromComponentOn(hud).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(hud);
    }

    private void BindBackpackDrawer()
    {
        EnvironmentObjectSpawnData backpackData =
            _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.BackpackDrawer);
        GameObject prefab = _prefabsStorage.Get(typeof(BackpackDrawer));
        IPositionAdapter positionAdapter = prefab.GetComponentInChildren<IPositionAdapter>(true);
        positionAdapter.Position = backpackData.Position;
        GameObject backpack = Container.InstantiatePrefab(prefab);
        backpack.gameObject.name = nameof(BackpackDrawer);
        Container.BindInterfacesAndSelfTo<BackpackDrawer>().FromComponentOn(backpack).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(backpack);
    }

    private void BindPlayer()
    {
        GameObject prefab = _prefabsStorage.Get(typeof(Player));
        EnvironmentObjectSpawnData playerSpawnData =
            _levelStaticData.GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId.Player);
        Vector3 position = playerSpawnData.Position;
        Quaternion rotation = playerSpawnData.Rotation;
        
        GameObject player = Container.InstantiatePrefab(prefab, position, rotation, null);
        player.name = nameof(Player);
        Container.BindInterfacesAndSelfTo<Player>().FromComponentOn(player).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(player);
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

    private void BindTimersPrincipal()
    {
        Debug.Log($"Instantiate TimersPrincipal start ");
        GameObject prefab = _prefabsStorage.Get(typeof(TimersPrincipal));
        GameObject timersPrincipal = Container.InstantiatePrefab(prefab, _hudTransform);
        Container.BindInterfacesAndSelfTo<TimersPrincipal>().FromComponentOn(timersPrincipal).AsSingle();
        _saveLoadStorage.RegisterInSaveLoadRepositories(timersPrincipal);
    }
    
    private void BindTray()
    {
        Container.BindInterfacesAndSelfTo<Tray>().AsSingle();
        Tray tray = Container.Resolve<Tray>();
        _saveLoadStorage.RegisterInSaveLoadRepositories(tray);
    }
}