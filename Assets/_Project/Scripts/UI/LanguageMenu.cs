using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class LanguageMenu : MonoBehaviour
    {
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private PointerListener _russianLanguage;
        [SerializeReference] private PointerListener _englishLanguage;
        [SerializeField] private PointerListener _back;

        private void OnEnable()
        {
            _russianLanguage.Click += OnRussianLanguageButton;
            _englishLanguage.Click += OnEnglishLanguageButton;
            _back.Click += OnBackButton;
        }

        private void OnDisable()
        {
            _russianLanguage.Click -= OnRussianLanguageButton;
            _englishLanguage.Click -= OnEnglishLanguageButton;
            _back.Click -= OnBackButton;
        }

        private void OnRussianLanguageButton(PointerEventData obj)
        {
            Debug.Log("Select russian language");
        }

        private void OnEnglishLanguageButton(PointerEventData obj)
        {
            Debug.Log("Select english language");
        }

        private void OnBackButton(PointerEventData data)
        {
            HideLanguageMenu();
            ShowMainMenu();
        }
        private void ShowMainMenu()
        {
            _mainMenu.gameObject.SetActive(true);
        }

        private void HideLanguageMenu()
        {
            gameObject.SetActive(false);
        }
    }
}
