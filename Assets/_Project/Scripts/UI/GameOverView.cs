using TMPro;
using UnityEngine;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _continueCanvasGroup;
    [SerializeReference] private TMP_Text _priceText;
    [SerializeField] private TMP_Text _walletAmount;
    private IWalletService _wallet;
    private int _petPrice;

    public void Construct(IWalletService wallet, int petPrice)
    {
        _wallet = wallet;
        _petPrice = petPrice;
    }
    private void OnEnable()
    {
        ValidateCurrencyCount();
        _priceText.text = _petPrice.ToString();
    }
    
    private void ValidateCurrencyCount()
    {
        Debug.Log($"Coins amount: {_wallet.GetAmount(CurrencyType.Coins)}");

        _walletAmount.text = _wallet.GetAmount(CurrencyType.Coins).ToString();
        
        if (_wallet.GetAmount(CurrencyType.Coins) < _petPrice)
            SetCanvasGroupValues(_continueCanvasGroup, .8f, false, false);
        else
            SetCanvasGroupValues(_continueCanvasGroup, 1f, true, true);
    }
    
    private void SetCanvasGroupValues(CanvasGroup canvasGroup, float alphaValue, bool interactable, bool blockRaycast)
    {
        canvasGroup.alpha = alphaValue;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = blockRaycast;
    }
}