#region

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Handles theme-based color updates for UI elements such as images and text.
    /// </summary>
    public class ThemeUI : MonoBehaviour
    {
        [SerializeField] private ColorType mColorType;
        [SerializeField] private ThemeConfigSO mThemeConfig;

        private Image mImage;
        private TextMeshProUGUI mText;

        private void Awake()
        {
            // Cache the Image or TextMeshProUGUI component and validate their existence.
            mImage = GetComponent<Image>();
            mText = GetComponent<TextMeshProUGUI>();

            if (mImage == null && mText == null)
            {
                Debug.LogError($"ThemeUI requires either Image or TextMeshProUGUI component on {gameObject.name}");
                enabled = false;
            }
        }

        private void OnEnable()
        {
            // Subscribe to theme change events and update the UI color.
            if (mThemeConfig != null)
            {
                mThemeConfig.OnThemeChanged += UpdateColor;
                UpdateColor();
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from theme change events to avoid memory leaks.
            if (mThemeConfig != null) mThemeConfig.OnThemeChanged -= UpdateColor;
        }

        /// <summary>
        ///     Updates the color of the associated Image or TextMeshProUGUI based on the current theme.
        /// </summary>
        private void UpdateColor()
        {
            var color = mThemeConfig.GetColor(mColorType);

            if (mImage != null)
                mImage.color = color;

            if (mText != null)
                mText.color = color;
        }
    }
}