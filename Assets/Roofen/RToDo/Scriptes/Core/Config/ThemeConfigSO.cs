#region

using System;
using UnityEngine;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     ScriptableObject that manages theme color configurations and dark/light mode switching
    /// </summary>
    [CreateAssetMenu(menuName = "RGame/RToDo/ThemeConfig")]
    public class ThemeConfigSO : ScriptableObject, IThemeConfig
    {
        private const string THEME_MODE_KEY = "ThemeMode";
        [SerializeField] private string mDarkBGColor = "1F1F1F";
        [SerializeField] private string mDarkTextColor = "FFFFFF";
        [SerializeField] private string mDarkSecondaryBGColor = "383838";
        [SerializeField] private string mDarkSecondaryTextColor = "77B0AC";

        [SerializeField] private string mLightBGColor = "FEFEFC";
        [SerializeField] private string mLightTextColor = "5A4633";
        [SerializeField] private string mLightSecondaryBGColor = "F7E9DA";
        [SerializeField] private string mLightSecondaryTextColor = "8B6E4A";

        private void OnEnable()
        {
            IsDarkMode = PlayerPrefs.GetInt(THEME_MODE_KEY, 0) == 1;
        }

        public event Action OnThemeChanged;
        public bool IsDarkMode { get; private set; }

        /// <summary>
        ///     Switches between dark/light mode and saves preference
        /// </summary>
        public void ToggleThemeMode()
        {
            IsDarkMode = !IsDarkMode;
            PlayerPrefs.SetInt(THEME_MODE_KEY, IsDarkMode ? 1 : 0);
            PlayerPrefs.Save();
            OnThemeChanged?.Invoke();
        }

        /// <summary>
        ///     Retrieves color value for specified type in current theme mode
        /// </summary>
        public Color GetColor(ColorType _colorType)
        {
            var hexColor = GetHexColor(_colorType);

            if (ColorUtility.TryParseHtmlString("#" + hexColor, out var color)) return color;

            Debug.LogError($"Failed to parse color: {hexColor}");
            return Color.black;
        }

        private string GetHexColor(ColorType _colorType)
        {
            return IsDarkMode ? GetDarkModeColor(_colorType) : GetLightModeColor(_colorType);
        }

        private string GetDarkModeColor(ColorType _colorType)
        {
            return _colorType switch
            {
                ColorType.Background => mDarkBGColor,
                ColorType.Text => mDarkTextColor,
                ColorType.SecondaryBackground => mDarkSecondaryBGColor,
                ColorType.SecondaryText => mDarkSecondaryTextColor,
                _ => throw new ArgumentOutOfRangeException(nameof(_colorType))
            };
        }

        private string GetLightModeColor(ColorType _colorType)
        {
            return _colorType switch
            {
                ColorType.Background => mLightBGColor,
                ColorType.Text => mLightTextColor,
                ColorType.SecondaryBackground => mLightSecondaryBGColor,
                ColorType.SecondaryText => mLightSecondaryTextColor,
                _ => throw new ArgumentOutOfRangeException(nameof(_colorType))
            };
        }
    }
}