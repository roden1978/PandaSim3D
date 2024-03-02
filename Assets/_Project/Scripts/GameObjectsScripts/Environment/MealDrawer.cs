using System;
using System.Linq;
using PlayerScripts;
using StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MealDrawer : ItemDrawer, ISavedProgress, IInitializable
{
    [SerializeField] private Transform _anchorPointTransform;
    protected override void ShowDialog()
    {
        if (ItemType != ItemType.None) return;

        Debug.Log("Click on plate");
        MealInventoryDialog dialog = DialogManager.ShowDialog<MealInventoryDialog>();
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
        
            if(mealType.Equals(ItemType.None)) return;
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
        if(stuff.Item.StuffSpecies == StuffSpecies.Meal)
        {
            Inventory.TryAddItem(this, stuff.Item, Extensions.OneItem);
            stuff.Position = AnchorPointTransform.position;
            //stuff.gameObject.SetActive(false);
            UnStack();
            Destroy(stuff.gameObject);
        }
        else
        {
            stuff.Position = stuff.StartPosition;
        }
    }

    public override void UnStack()
    {
        ItemType = ItemType.None;
        SaveLoadService.SaveProgress();
    }

    public void Initialize()
    {
        //AnchorPointTransform = _anchorPointTransform;
    }
}