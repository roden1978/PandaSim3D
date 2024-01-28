using System;
using System.Collections.Generic;
using System.Linq;
using CustomEventBus.Signals;
using UnityEngine;
using Zenject;

public class WalletService : IWalletService, IInitializable
{
    private readonly IEventBus _eventBus;
    private readonly Dictionary<int, long> _wallets = new();

    private List<int> _walletTypes = new();

    // currency, old value, new value

    public WalletService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Initialize()
    {
        ClearWallets();
    }

    public long GetAmount(CurrencyType currency)
    {
        _wallets.TryGetValue((int)currency, out long value);
        return value;
    }

    public void AddAmount(CurrencyType currency, long amount) =>
        SetAmount(currency, GetAmount(currency) + amount);

    public bool TrySpend(CurrencyType currency, long price)
    {
        long amount = GetAmount(currency);

        if (amount < price)
            return false;

        amount -= price;
        SetAmount(currency, amount);
        return true;
    }

    public void LoadProgress(PlayerProgress progress)
    {
        ClearWallets();

        Dictionary<int, long> savedValues = progress.WalletsData.ToDictionary();

        foreach (var pair in savedValues)
        {
            int currencyTypeCode = pair.Key;
            long currencyAmount = pair.Value;

            if (!Enum.IsDefined(typeof(CurrencyType), currencyTypeCode))
            {
                Debug.LogError(
                    $"Unknown currency type code {currencyTypeCode} with amount {currencyAmount} on progress loading found! May be some old currency.");
                continue;
            }

            CurrencyType currencyType = (CurrencyType)currencyTypeCode;

            if (currencyType == CurrencyType.None)
            {
                Debug.LogError($"None currency type on progress loading found!");
                continue;
            }

            SetAmount(currencyType, currencyAmount);
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        playerProgress.WalletsData = new WalletsData(_wallets);
    }

    private void ClearWallets()
    {
        _wallets.Clear();

        _walletTypes = Enum.GetValues(typeof(CurrencyType)).Cast<int>().ToList();

        foreach (int type in _walletTypes)
            _wallets.Add(type, 0);
    }

    private void SetAmount(CurrencyType currency, long amount)
    {
        long oldValue = _wallets[(int)currency];

        _wallets[(int)currency] = amount;
        _eventBus.Invoke(new CoinsViewTextUpdateSignal(currency, amount, oldValue));
    }
}