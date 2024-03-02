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

    protected GameObject View;
    
    protected Transform AnchorPointTransform;
    protected DialogManager DialogManager => _dialogManager;
    protected ItemType ItemType = ItemType.None;
    protected IInventory Inventory;
    protected ISaveLoadService SaveLoadService;
    private readonly Dictionary<string, Stuff> _cachedItems = new();
    private DialogManager _dialogManager;
    private IAssetProvider _assetProvider;
    private IPositionAdapter _positionAdapter;

    [Inject]
    public void Contruct(DialogManager dialogManager, IAssetProvider assetProvider, IInventory inventory,
        ISaveLoadService saveLoadService, Player player)
    {
        _dialogManager = dialogManager;
        _assetProvider = assetProvider;
        Inventory = inventory;
        SaveLoadService = saveLoadService;
        _positionAdapter = player;
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

        await InstantiateItem(itemName);
    }

    protected async UniTask<Stuff> InstantiateItem(string itemName)
    {
        Stuff stuff;
        if (TryGetItemFromCache(itemName, out Stuff cached))
        {
            stuff = InstantiateStuff(cached.gameObject);
        }
        else
        {
            UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(itemName);

            await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
            GameObject prefab = await result;
            AddToItemCache(prefab.name, prefab.GetComponent<Stuff>());
            stuff = InstantiateStuff(prefab);
        }
        
        stuff.Construct(this, _positionAdapter);
        stuff.StartPosition = AnchorPointTransform.position;
        stuff.AddLastStack(this);
        
        _assetProvider.ReleaseAssetsByLabel(itemName);
        return stuff;
    }

    private Stuff InstantiateStuff(GameObject prefab)
    {
        return Instantiate(prefab, AnchorPointTransform.position, Quaternion.identity,
            AnchorPointTransform).GetComponent<Stuff>();
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

    protected void ViewSetActive(bool value)
    {
        View.SetActive(value);
    }

    public abstract void Stack(Stuff stuff);

    public abstract void UnStack();

}