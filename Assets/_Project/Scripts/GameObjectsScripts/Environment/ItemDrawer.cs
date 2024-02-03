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
    

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    protected Transform AnchorPointTransform;
    protected DialogManager DialogManager => _dialogManager;

    protected ItemType ItemType = ItemType.None;
    /*{
        get => _itemType;
        set => _itemType = value;
    }*/

    private DialogManager _dialogManager;
    //private ItemType _itemType = ItemType.None;
    private IAssetProvider _assetProvider;

    private readonly Dictionary<string, GameObject> _cachedItems = new();
    private IInventory _inventory;
    private ISaveLoadService _saveLoadService;

    [Inject]
    public void Contruct(DialogManager dialogManager, IAssetProvider assetProvider, IInventory inventory,
        ISaveLoadService saveLoadService)
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
        ItemType = type;
        string itemName = Enum.GetName(typeof(ItemType), (int)type);

        if (GetItemFromCache(itemName) is null)
        {
            await InstantiateItem(itemName);
        }
        else
        {
            GameObject item = GetItemFromCache(itemName);
            item.SetActive(true);
        }
    }

    protected async UniTask<Stuff> InstantiateItem(string itemName)
    {
        UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(itemName);

        await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
        GameObject prefab = await result;

        Stuff stuff = Instantiate(prefab, AnchorPointTransform.position, Quaternion.identity,
                AnchorPointTransform)
            .GetComponent<Stuff>();

        AddToMealCache(stuff.Item.Name, stuff.gameObject);

        stuff.Construct(this);

        _assetProvider.ReleaseAssetsByLabel(itemName);
        return stuff;
    }

    private void AddToMealCache(string itemName, GameObject item)
    {
        _cachedItems.TryAdd(itemName, item);
    }

    private GameObject GetItemFromCache(string itemName)
    {
        return _cachedItems.TryGetValue(itemName, out GameObject item) ? item : null;
    }

    public void Stack(Stuff stuff)
    {
        _inventory.TryAddItem(this, stuff.Item, Extensions.OneItem);
        stuff.gameObject.transform.position = AnchorPointTransform.position;
        stuff.gameObject.SetActive(false);
        UnStack(stuff);
    }

    public void UnStack(Stuff stuff)
    {
        ItemType = ItemType.None;
        _saveLoadService.SaveProgress();
    }
}