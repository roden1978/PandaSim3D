public class BuyItemSignal
{
    public Item Item { get; }
    public object Sender { get; }
    public int Value { get; }

    public BuyItemSignal(object sender, Item item, int value)
    {
        Item = item;
        Sender = sender;
        Value = value;
    }
}