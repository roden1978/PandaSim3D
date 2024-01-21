using System;
using System.Linq;
using PlayerScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snowman : MonoBehaviour, ISavedProgress
{
    [SerializeField] private SnowmanHead _snowmanHead;
    public void LoadProgress(PlayerProgress playerProgress)
    {
        //TODO: Implement snowman data load;
    }

    public void SaveProgress(PlayerProgress persistentPlayerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState room = persistentPlayerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
        if (room is not null)
            room.SnowmanDecor.Type = _snowmanHead.DecorType;
        else
            throw new ArgumentException($"Room not found {currentRoomName}");
    }
}