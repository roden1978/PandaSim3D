using System;
using System.Linq;
using Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;

public class ToysDrawer : ItemDrawer, ISavedProgress, IInitializable
{
    [SerializeField] private Transform _anchorPointTransform;
    [SerializeField] private GameObject _drawerView;
    private bool _isFull;
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
        _drawerView.SetActive(true);
        _isFull = true;
    }
    
    protected override void ShowDialog()
    {
        if (ItemType != ItemType.None) return;

        Debug.Log("Click on plate");
        ToysInventoryDialog dialog = DialogManager.ShowDialog<ToysInventoryDialog>();
        dialog.UpdateInventoryView();
    }

    public async void LoadProgress(PlayerProgress playerProgress)
    {
        AnchorPointTransform = _anchorPointTransform;
        
        RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == AssetPaths.RoomSceneName.ToString());

        if (roomState is not null && roomState.Ball)
        {
            ItemType = ItemType.Ball;
            string itemName = Enum.GetName(typeof(ItemType), (int)ItemType.Ball);
            Stuff stuff = await InstantiateItem(itemName);
            stuff.Position = stuff.StartPosition = _anchorPointTransform.position;
            _isFull = true;
            stuff.AddLastStack(this);
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == AssetPaths.RoomSceneName.ToString());
        if (room is not null)
        {
            room.Ball = _isFull;
        }
        else
            playerProgress.RoomsData.Rooms.Add(new RoomState
            {
                Ball = _isFull,
                Name = AssetPaths.RoomSceneName.ToString()
            });
    }

    public override void Stack(Stuff stuff)
    {
        /*Inventory.TryAddItem(this, stuff.Item, Extensions.OneItem);
        stuff.StartPosition = stuff.Position = AnchorPointTransform.position;
        stuff.LastStack.UnStack();
        stuff.AddLastStack(this);
        SaveLoadService.SaveProgress();
        Destroy(stuff.gameObject);*/
    }

    public override void UnStack()
    {
        //ItemType = ItemType.None;
    }

    public void Initialize()
    {
    }

    private void OnDestroy()
    {
        _eventBus.Unsubscribe<BoughtBallSignal>(OnBoughtBall);
    }
}