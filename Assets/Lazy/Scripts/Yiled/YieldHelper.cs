using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YieldHelper
{
    /// <summary>
    /// 静态WaitForEndOfFrame对象，重复使用，避免创建新对象
    /// </summary>
    public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

    /// <summary>
    /// 等待一定时间，执行
    /// </summary>
    /// <param name="totalTime"></param>
    /// <param name="ignorTimeScake"></param>
    /// <returns></returns>
    public static IEnumerator WaitForSeconds(float totalTime, bool ignorTimeScake = false)
    {
        float time = 0;
        while (time < totalTime)
        {
            time += (ignorTimeScake ? Time.unscaledDeltaTime : Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// 等待一定帧数，执行
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static IEnumerator WaitForFrame(int i)
    {
        int count = 0;
        while (count < i)
        {
            yield return null;
            count++;
        }
    }

}
