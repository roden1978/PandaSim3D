using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MealDrawer : ItemDrawer, ISavedProgress
{
    protected override void ShowDialog()
    {
        if (ItemType != ItemType.None) return;

        Debug.Log("Click on plate");
        MealInventoryDialog dialog = DialogManager.ShowDialog<MealInventoryDialog>();
        dialog.UpdateInventoryView();
    }
    public async void LoadProgress(PlayerProgress playerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
        
        if (roomState is not null)
        {
            ItemType mealType = roomState.MealData.Type; 
        
            if(mealType.Equals(ItemType.None)) return;
            ItemType = mealType;
            string mealName = Enum.GetName(typeof(ItemType), (int)mealType);
            Stuff stuff = await InstantiateItem(mealName);
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
                MealData = new MealData
                {
                    Type = ItemType
                },
                Name = currentRoomName
            });
    }
}