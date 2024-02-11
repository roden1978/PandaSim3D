using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class BackpackDrawer : ItemDrawer, ISavedProgress, IInitializable
{
    [SerializeField] private Transform _anchorPointTransform;

    protected override void ShowDialog()
    {
        if (ItemType != ItemType.None) return;

        Debug.Log("Click on plate");
        BackpackInventoryDialog dialog = DialogManager.ShowDialog<BackpackInventoryDialog>();
        dialog.UpdateInventoryView();
    }

    public async void LoadProgress(PlayerProgress playerProgress)
    {
        AnchorPointTransform = _anchorPointTransform;
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);

        if (roomState is not null)
        {
            ItemType mealType = roomState.ItemData.Type;

            if (mealType.Equals(ItemType.None)) return;
            ItemType = mealType;
            string mealName = Enum.GetName(typeof(ItemType), (int)mealType);
            Stuff stuff = await InstantiateItem(mealName);
            stuff.AddLastStack(this);
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
        if (room is not null)
        {
            room.ItemData ??= new ItemData
            {
                Type = ItemType,
            };

            room.ItemData.Type = ItemType;
        }
        else
            playerProgress.RoomsData.Rooms.Add(new RoomState
            {
                ItemData = new ItemData
                {
                    Type = ItemType
                },
                Name = currentRoomName
            });
    }

    public override void Stack(Stuff stuff)
    {
        Inventory.TryAddItem(this, stuff.Item, Extensions.OneItem);
        stuff.StartPosition = stuff.Position = AnchorPointTransform.position;
        stuff.LastStack.UnStack();
        stuff.AddLastStack(this);
        SaveLoadService.SaveProgress();
        Destroy(stuff.gameObject);
    }

    public override void UnStack()
    {
        ItemType = ItemType.None;
    }

    public void Initialize()
    {
    }
}