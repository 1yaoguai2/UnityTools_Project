#region

using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     UI panel for displaying the main menu and handling user interactions
    /// </summary>
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mCanvasGroup;
        [SerializeField] private TextMeshProUGUI mThemeButtonText;
        [SerializeField] private ThemeConfigSO mThemeConfig;

        /// <summary>
        ///     Initializes the theme button text on start
        /// </summary>
        private void Start()
        {
            UpdateThemeButtonText();
        }

        public event Action OnReturnRequested;
        public event Action OnCompleteListRequested;

        /// <summary>
        ///     Fades in the menu panel and enables interaction
        /// </summary>
        public void Show()
        {
            mCanvasGroup.DOFade(1, 0.3f).OnComplete(() =>
            {
                mCanvasGroup.interactable = true;
                mCanvasGroup.blocksRaycasts = true;
            });
        }

        /// <summary>
        ///     Fades out the menu panel and disables interaction
        /// </summary>
        public void Hide()
        {
            mCanvasGroup.DOFade(0, 0.3f);
            mCanvasGroup.interactable = false;
            mCanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        ///     Handles return button click event
        /// </summary>
        public void OnReturnClicked()
        {
            OnReturnRequested?.Invoke();
        }

        /// <summary>
        ///     Handles complete list button click event
        /// </summary>
        public void OnCompleteListClicked()
        {
            OnCompleteListRequested?.Invoke();
        }

        /// <summary>
        ///     Toggles between dark and light theme modes
        /// </summary>
        public void OnThemeToggleClicked()
        {
            mThemeConfig.ToggleThemeMode();
            UpdateThemeButtonText();
        }

        /// <summary>
        ///     Opens the documentation URL in the default browser
        /// </summary>
        public void OnDocumentationClicked()
        {
            Application.OpenURL("https://roofen-game.gitbook.io/roofen-game");
        }

        /// <summary>
        ///     Updates the theme button text based on the current theme mode
        /// </summary>
        private void UpdateThemeButtonText()
        {
            mThemeButtonText.text = mThemeConfig.IsDarkMode ? "Light Mode" : "Dark Mode";
        }
    }
}