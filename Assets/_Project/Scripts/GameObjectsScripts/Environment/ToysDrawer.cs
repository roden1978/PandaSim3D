using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using PlayerScripts;
using StaticData;
using UnityEngine;
using Zenject;

public class ToysDrawer : ItemDrawer, ISavedProgress
{
    [SerializeField] private Transform _anchorPointTransform;
    [SerializeField] private GameObject _drawerView;
    private bool _isFull;
    private bool _isDrawerEnabled;
    private IEventBus _eventBus;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
        View = _drawerView;
    }

    private void OnEnable()
    {
        _eventBus.Subscribe<BoughtBallSignal>(OnBoughtBall);
    }

    private void OnBoughtBall(BoughtBallSignal signal)
    {
        DrawerSetActive(true);
        _isFull = true;
        SaveLoadService.SaveProgress();
    }

    public void DrawerSetActive(bool value)
    {
        ViewSetActive(value);
        _isDrawerEnabled = value;
    }

    protected override void ShowDialog()
    {
        ToysInventoryDialog dialog = DialogManager.ShowDialog<ToysInventoryDialog>();
        dialog.UpdateInventoryView();
    }

    public async void LoadProgress(PlayerProgress playerProgress)
    {
        AnchorPointTransform = _anchorPointTransform;

        RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == AssetPaths.RoomSceneName.ToString());

        if (roomState is null) return;
        
        if (roomState.Ball)
        {
            ItemType = ItemType.Ball;
            string itemName = Enum.GetName(typeof(ItemType), (int)ItemType.Ball);
            Stuff stuff = await InstantiateItem(itemName);
            stuff.Position = stuff.StartPosition = _anchorPointTransform.position;
            _isFull = true;
            stuff.AddLastStack(this);
        }

        if (roomState.ToysDrawer)
            DrawerSetActive(true);
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == AssetPaths.RoomSceneName.ToString());
        if (room is null)
            playerProgress.RoomsData.Rooms.Add(new RoomState
            {
                Ball = _isFull,
                ToysDrawer = _isDrawerEnabled,
                Name = AssetPaths.RoomSceneName.ToString()
            });
        else
        {
            room.Ball = _isFull;
            room.ToysDrawer = _isDrawerEnabled;
        }
            
    }

    public override void Stack(Stuff stuff)
    {
    }

    public override void UnStack()
    {
    }

    private void OnDestroy()
    {
        _eventBus.Unsubscribe<BoughtBallSignal>(OnBoughtBall);
    }
}