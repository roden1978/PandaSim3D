using System;
using System.Collections.Generic;
using System.Linq;
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

    [Inject]
    public void Contruct(DialogManager dialogManager, IAssetProvider assetProvider, IInventory inventory)
    {
        _dialogManager = dialogManager;
        _assetProvider = assetProvider;
        _inventory = inventory;
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
        string mealName = Enum.GetName(typeof(ItemType), (int)type);

        if (GetMealFromCache(mealName) is null)
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
        else
        {
            GameObject go = GetMealFromCache(mealName);
            go.SetActive(true);
        }

        _itemType = type;
    }

    public void RemoveMeal()
    {
        _itemType = ItemType.None;
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
    }

    public void UnStack(Stuff stuff)
    {
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        //TODO: Implement plate data loading and spawn food on plate
    }

    public void SaveProgress(PlayerProgress persistentPlayerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState room = persistentPlayerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
        if (room is not null)
            room.FoodData.Type = _itemType;
        else
            throw new ArgumentException($"Room not found {currentRoomName}");
    }
}