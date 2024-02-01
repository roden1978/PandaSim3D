using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public ItemType ItemType;
    public Image Icon;
    public TMP_Text ValueText;
    public int InventorySlotId = int.MaxValue;
}