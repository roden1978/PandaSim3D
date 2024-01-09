using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class About : MonoBehaviour
    {
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private Button _back;

        private void Start()
        {
            _back.onClick.AddListener(OnBackButton);
        }

        private void OnBackButton()
        {
            HideSettings();
            ShowMainMenu();
        }
        private void ShowMainMenu()
        {
            _mainMenu.gameObject.SetActive(true);
        }

        private void HideSettings()
        {
            gameObject.SetActive(false);
        }
    }
}
