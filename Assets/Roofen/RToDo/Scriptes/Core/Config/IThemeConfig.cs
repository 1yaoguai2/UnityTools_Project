#region

using System;
using UnityEngine;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Defines color types used in the application's theme system
    /// </summary>
    public enum ColorType
    {
        Background,
        Text,
        SecondaryBackground,
        SecondaryText
    }

    /// <summary>
    ///     Interface for theme configuration management, handling color schemes and dark/light mode switching
    /// </summary>
    public interface IThemeConfig
    {
        bool IsDarkMode { get; }

        void ToggleThemeMode();

        Color GetColor(ColorType _colorType);

        event Action OnThemeChanged;
    }
}