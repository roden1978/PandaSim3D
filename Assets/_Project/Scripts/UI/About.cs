using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class About : MonoBehaviour
    {
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private PointerListener _back;

        private void OnEnable()
        {
            _back.Click += OnBackButton;
        }

        private void OnDisable()
        {
            _back.Click -= OnBackButton;
        }

        private void OnBackButton(PointerEventData data)
        {
            HideAbout();
            ShowMainMenu();
        }
        private void ShowMainMenu()
        {
            _mainMenu.gameObject.SetActive(true);
        }

        private void HideAbout()
        {
            gameObject.SetActive(false);
        }
    }
}