using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClothsDrawer : ItemDrawer, ISavedProgress
{
    protected override void ShowDialog()
    {
        if (ItemType != ItemType.None) return;

        Debug.Log("Click on ClothsDrawer");
        ClothsInventoryDialog dialog = DialogManager.ShowDialog<ClothsInventoryDialog>();
        dialog.UpdateInventoryView();
    }
    
    public async void LoadProgress(PlayerProgress playerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
        
        if (roomState is not null)
        {
            ItemType clothsType = roomState.ClothsData.Type; 
        
            if(clothsType.Equals(ItemType.None)) return;
            ItemType = clothsType;
            string clothName = Enum.GetName(typeof(ItemType), (int)clothsType);
            Stuff stuff = await InstantiateItem(clothName);
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
        if (room is not null)
            room.MealData.Type = ItemType;
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