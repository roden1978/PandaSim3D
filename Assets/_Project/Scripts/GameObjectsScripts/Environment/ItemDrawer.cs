using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using PlayerScripts;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public abstract class ItemDrawer : MonoBehaviour, IPositionAdapter, IPointerClickHandler, IStack
{
    [SerializeField] private Transform _anchorPointTransform;

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    protected DialogManager DialogManager => _dialogManager;
    protected ItemType ItemType { get=>_itemType; set =>_itemType = value; }
    
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
        ShowDialog();
    }
    protected abstract void ShowDialog();
    
    public async void InstantiateItemByType(ItemType type)
    {
        _itemType = type;
        string mealName = Enum.GetName(typeof(ItemType), (int)type);

        if (GetMealFromCache(mealName) is null)
        {
            await InstantiateItem(mealName);
        }
        else
        {
            GameObject go = GetMealFromCache(mealName);
            go.SetActive(true);
        }
    }

    protected async UniTask<Stuff> InstantiateItem(string mealName)
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
        return stuff;
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
}