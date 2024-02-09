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
    private readonly Dictionary<string, Stuff> _cachedItems = new();
    private DialogManager _dialogManager;
    private IAssetProvider _assetProvider;
    protected IInventory Inventory;
    protected ISaveLoadService SaveLoadService;

    [Inject]
    public void Contruct(DialogManager dialogManager, IAssetProvider assetProvider, IInventory inventory,
        ISaveLoadService saveLoadService)
    {
        _dialogManager = dialogManager;
        _assetProvider = assetProvider;
        Inventory = inventory;
        SaveLoadService = saveLoadService;
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

        Stuff stuff = await InstantiateItem(itemName);
        
        stuff.StartPosition = AnchorPointTransform.position;
        stuff.AddLastStack(this);

        stuff.Construct(this);
    }

    protected async UniTask<Stuff> InstantiateItem(string itemName)
    {
        Stuff stuff;
        if (TryGetItemFromCache(itemName, out Stuff cached))
        {
            stuff = Instantiate(cached, AnchorPointTransform.position, Quaternion.identity,
                AnchorPointTransform).GetComponent<Stuff>();
        }
        else
        {
            UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(itemName);

            await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
            GameObject prefab = await result;
            AddToItemCache(prefab.name, prefab.GetComponent<Stuff>());
            stuff = Instantiate(prefab, AnchorPointTransform.position, Quaternion.identity,
                AnchorPointTransform).GetComponent<Stuff>();
        }
        
        _assetProvider.ReleaseAssetsByLabel(itemName);
        return stuff;
    }

    private bool AddToItemCache(string itemName, Stuff item)
    {
        return _cachedItems.TryAdd(itemName, item);
        //Debug.Log($"Add stuff to cache result: {result}");
    }

    private bool TryGetItemFromCache(string itemName, out Stuff item)
    {
        return _cachedItems.TryGetValue(itemName, out item);
    }

    public abstract void Stack(Stuff stuff);

    public abstract void UnStack();

}