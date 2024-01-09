using Services.SaveLoad.PlayerProgress;
using Zenject;

public class SaveLoadStorageWalletInstaller : MonoInstaller
{
    private ISaveLoadStorage _saveLoadStorage;
    private IWalletService _walletService;

    [Inject]
    public void Construct(ISaveLoadStorage saveLoadStorage,
        IWalletService walletService)
    {
        _saveLoadStorage = saveLoadStorage;
        _walletService = walletService;
    }

    public override void InstallBindings()
    {
        RegisterWallet();
    }

    private void RegisterWallet()
    {
        _saveLoadStorage.RegisterInSaveLoadRepositories(_walletService);
    }
}