using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unity扩展类
/// Image
/// </summary>
public static class ImageExtensions
{
    /// <summary>
    /// Set the alpha value of an Image component
    /// 设置Image组件的透明度
    /// use: image.SetAlpha(0.5f);
    /// </summary>
    /// <param name="image">The Image component</param>
    /// <param name="alpha">Alpha value (0-1)</param>
    public static void SetAlpha(this Image image, float alpha)
    {
        Color color = image.color;
        color.a = Mathf.Clamp01(alpha);
        image.color = color;
    }

    /// <summary>
    /// Fade the image to target alpha value
    /// 渐变图片到目标透明度
    /// use: StartCoroutine(image.FadeTo(0f, 1f)); // 1秒内淡出 (Fade out in 1 second)
    /// </summary>
    /// <param name="image">The Image component</param>
    /// <param name="targetAlpha">Target alpha value (0-1)</param>
    /// <param name="duration">Fade duration in seconds</param>
    /// <returns>Coroutine IEnumerator</returns>
    public static IEnumerator FadeTo(this Image image, float targetAlpha, float duration)
    {
        float startAlpha = image.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            image.SetAlpha(currentAlpha);
            yield return null;
        }

        image.SetAlpha(targetAlpha);
    }
}