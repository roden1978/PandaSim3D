using System;
using CustomEventBus;
using CustomEventBus.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    [SerializeField] private Image _frame;
    [SerializeField] private PointerListener _listener;
    public UIItem UIItem;
    public bool IsActive;
    public int Id => _id;
    private int _id;
    private ISlotChanger _slotChanger;

    public void Construct(ISlotChanger slotChanger, int id)
    {
        _slotChanger = slotChanger;
        _id = id;
    }

    private void OnEnable()
    {
        _listener.Click += OnSlotClick;
    }

    private void OnDisable()
    {
        _listener.Click += OnSlotClick;
    }

    private void OnSlotClick(PointerEventData obj)
    {
        _slotChanger.ChangeActiveSlot(_id);
    }

    public void ActivateFrame(bool value)
    {
        _frame.gameObject.SetActive(value);
    }
}