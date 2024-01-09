    public interface IWalletService : ISavedProgress
    {
        long GetAmount(CurrencyType currency);
        void AddAmount(CurrencyType currency, long amount);
        bool TrySpend(CurrencyType currency, long price);
    }
