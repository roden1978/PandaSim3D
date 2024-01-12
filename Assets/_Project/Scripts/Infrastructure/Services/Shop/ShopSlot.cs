using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class ShopSlot: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PointerListener _select;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _price;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private CanvasGroup _frameCanvasGroup;
    public int Price => _item.Price;
    private Item _item;
    private const int Value = 1;
    
    private IEventBus _eventBus;

    public void Construct(IEventBus eventBus, Item item)
    {
        _eventBus = eventBus;
        _item = item;
        _icon.sprite = _item.Sprite;
        _name.text = _item.Name; 
        _price.text =  Convert.ToString(_item.Price);
    }

    private void OnEnable()
    {
        _select.Click += BuyItem;
    }

    private void BuyItem(PointerEventData obj)
   {
       _eventBus.Invoke(new BuyItemSignal(this, _item, Value));
   }

    private void OnDisable()
    {
        _select.Click -= BuyItem;
    }

    public void DisableSlot()
    {
        _canvasGroup.DOFade(.5f, 0.5f);;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void EnableSlot()
    {
        _canvasGroup.DOFade(1, 0.5f);;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _frameCanvasGroup.DOFade(1, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _frameCanvasGroup.DOFade(0, 0.5f);
    }
}