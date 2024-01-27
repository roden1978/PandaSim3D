public class Slot
{
    public int Id;
    public bool IsEmpty => InventoryItem is null;
    public int ItemAmount;
    public Item InventoryItem;
}