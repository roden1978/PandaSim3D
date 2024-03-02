using System;
using System.Linq;
using Infrastructure.AssetManagement;
using PlayerScripts;
using StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClothsDrawer : ItemDrawer, ISavedProgress
{
    [SerializeField] private Transform _anchorPointTransform;

    protected override void ShowDialog()
    {
        if (ItemType != ItemType.None) return;

        Debug.Log("Click on ClothsDrawer");
        ClothsInventoryDialog dialog = DialogManager.ShowDialog<ClothsInventoryDialog>();
        dialog.UpdateInventoryView();
    }

    public async void LoadProgress(PlayerProgress playerProgress)
    {
        AnchorPointTransform = _anchorPointTransform;
        string currentRoomName = SceneManager.GetActiveScene().name;
        if (currentRoomName == AssetPaths.RoomSceneName.ToString())
        {
            RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
                x.Name == currentRoomName);

            if (roomState is not null)
            {
                ItemType clothsType = roomState.ClothsData.Type;

                if (clothsType.Equals(ItemType.None)) return;
                ItemType = clothsType;
                string clothName = Enum.GetName(typeof(ItemType), (int)clothsType);
                Stuff stuff = await InstantiateItem(clothName);
                stuff.AddLastStack(this);
            }
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        if (currentRoomName == AssetPaths.RoomSceneName.ToString())
        {
            RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
                x.Name == currentRoomName);
            if (room is not null)
            {
                room.ClothsData ??= new ClothsData
                {
                    Type = ItemType,
                };

                room.ClothsData.Type = ItemType;
            }
            else
                playerProgress.RoomsData.Rooms.Add(new RoomState
                {
                    ClothsData = new ClothsData
                    {
                        Type = ItemType
                    },
                    Name = currentRoomName
                });
        }
    }

    public override void Stack(Stuff stuff)
    {
        if (stuff.Item.StuffSpecies is StuffSpecies.Cloths or StuffSpecies.Decor)
        {
            Inventory.TryAddItem(this, stuff.Item, Extensions.OneItem);
            stuff.StartPosition = stuff.Position = AnchorPointTransform.position;
            stuff.transform.parent = _anchorPointTransform;
            stuff.LastStack.UnStack();
            stuff.AddLastStack(this);
            SaveLoadService.SaveProgress();
            Destroy(stuff.gameObject);
        }
        else
        {
            stuff.Position = stuff.StartPosition;
        }
    }

    public override void UnStack()
    {
        Debug.Log("Override Virtual unstack method");
        ItemType = ItemType.None;
    }
}