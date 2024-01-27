using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using PlayerScripts;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;


public class Plate : MonoBehaviour, IPositionAdapter, IPointerClickHandler, IStack, ISavedProgress
{
    [SerializeField] private Transform _anchorPointTransform;

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    private DialogManager _dialogManager;
    private ItemType _itemType = ItemType.None;
    private IAssetProvider _assetProvider;

    private readonly Dictionary<string, GameObject> _cachedMeals = new();
    private IInventory _inventory;
    private ISaveLoadService _saveLoadService;

    [Inject]
    public void Contruct(DialogManager dialogManager, IAssetProvider assetProvider, IInventory inventory, ISaveLoadService saveLoadService)
    {
        _dialogManager = dialogManager;
        _assetProvider = assetProvider;
        _inventory = inventory;
        _saveLoadService = saveLoadService;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_itemType != ItemType.None) return;

        Debug.Log("Click on plate");
        InventoryDialog dialog = _dialogManager.ShowDialog<InventoryDialog>();
        dialog.UpdateInventoryView();
    }

    public async void InstantiateMeal(ItemType type)
    {
        _itemType = type;
        string mealName = Enum.GetName(typeof(ItemType), (int)type);

        if (GetMealFromCache(mealName) is null)
        {
            await InstantiateMeal(mealName);
        }
        else
        {
            GameObject go = GetMealFromCache(mealName);
            go.SetActive(true);
        }
    }

    private async Task InstantiateMeal(string mealName)
    {
        UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(mealName);

        await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
        GameObject prefab = await result;

        Stuff stuff = Instantiate(prefab, _anchorPointTransform.position, Quaternion.identity,
                _anchorPointTransform)
            .GetComponent<Stuff>();

        AddToMealCache(stuff.Item.Name, stuff.gameObject);

        stuff.Construct(this);

        _assetProvider.ReleaseAssetsByLabel(mealName);
    }

    private void AddToMealCache(string mealName, GameObject meal)
    {
        _cachedMeals.TryAdd(mealName, meal);
    }

    private GameObject GetMealFromCache(string mealName)
    {
        return _cachedMeals.TryGetValue(mealName, out GameObject go) ? go : null;
    }

    public void Stack(Stuff stuff)
    {
        _inventory.TryAddItem(this, stuff.Item, Extensions.OneItem);
        stuff.gameObject.transform.position = _anchorPointTransform.position;
        stuff.gameObject.SetActive(false);
        UnStack(stuff);
    }

    public void UnStack(Stuff stuff)
    {
        _itemType = ItemType.None;
        _saveLoadService.SaveProgress();
    }

    public async void LoadProgress(PlayerProgress playerProgress)
    {
        //TODO: Implement plate data loading and spawn food on plate
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
        
        if (roomState is not null)
        {
            ItemType melaType = roomState.MealData.Type; 
        
            if(melaType.Equals(ItemType.None)) return;
            _itemType = melaType;
            string mealName = Enum.GetName(typeof(ItemType), (int)melaType);
            await InstantiateMeal(mealName);
        }
    }

    public void SaveProgress(PlayerProgress persistentPlayerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState room = persistentPlayerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
        if (room is not null)
            room.MealData.Type = _itemType;
        else
            persistentPlayerProgress.RoomsData.Rooms.Add(new RoomState
            {
                MealData = new MealData
                {
                    Type = _itemType
                },
                Name = currentRoomName
            });
    }
}