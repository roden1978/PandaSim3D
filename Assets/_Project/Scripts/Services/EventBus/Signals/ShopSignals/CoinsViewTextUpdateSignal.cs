namespace CustomEventBus.Signals
{
    public class CoinsViewTextUpdateSignal
    {
        public CurrencyType CurrencyType;
        public long NewValue;
        public long OldValue;
        public CoinsViewTextUpdateSignal(CurrencyType currencyType, long newValue, long oldValue)
        {
            CurrencyType = currencyType;
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}