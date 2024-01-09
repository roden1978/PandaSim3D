using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopSlot: MonoBehaviour
{
    private string _name;
    [SerializeField] private PointerListener _select;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _gold;
    public int Price => _item.Price;
    private Item _item;
    private const int Value = 1;
    
    private IEventBus _eventBus;

    public void Construct(IEventBus eventBus, Item item)
    {
        _eventBus = eventBus;
        _item = item;
        _icon.sprite = _item.Sprite;
        _gold.text =  Convert.ToString(_item.Price);
    }

    private void OnEnable()
    {
        _select.Click += BuyItem;
    }

    private void BuyItem(PointerEventData obj)
   {
       _eventBus.Invoke(new BuyItemSignal(this, _item, Value));
       /*if (_walletService.TrySpend(CurrencyType.Coins, _item.Price))
       {
           _inventory.TryAddItem(this, _item, 1);
           Debug.Log($"Was spend {_item.Price}");
       }*/
   }

    private void OnDisable()
    {
        _select.Click -= BuyItem;
    }

    public void DisableSlot()
    {
        _canvasGroup.alpha = 0.5f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void EnableSlot()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
}