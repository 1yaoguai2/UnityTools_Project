using UnityEngine;
using System.Collections;
using System;

public static class MonoBehaviourExtensions
{
    #region Delay Actions
    /// <summary>
    /// 延迟执行操作(时间受到TimeScale影响)
    /// Delay an action
    /// </summary>
    public static Coroutine Delay(this MonoBehaviour mb, float delay, Action action)
    {
        return mb.StartCoroutine(DelayCoroutine(delay, action));
    }
    private static IEnumerator DelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    /// <summary>
    /// 使用真实时间延迟执行（时间不受TimeScale影响）
    /// Delay using real time (ignores TimeScale)
    /// </summary>
    public static Coroutine DelayRealtime(this MonoBehaviour mb, float delay, Action action)
    {
        return mb.StartCoroutine(DelayRealtimeCoroutine(delay, action));
    }

    private static IEnumerator DelayRealtimeCoroutine(float delay, Action action)
    {
        yield return new WaitForSecondsRealtime(delay);
        action?.Invoke();
    }
    #endregion

    #region Component Operations
    /// <summary>
    /// 获取或添加组件
    /// Get or add component
    /// </summary>
    public static T GetOrAddComponent<T>(this MonoBehaviour mb) where T : Component
    {
        T component = mb.GetComponent<T>();
        if (component == null)
        {
            component = mb.gameObject.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// 尝试获取组件，如果不存在则返回false
    /// Try get component, returns false if not found
    /// </summary>
    public static bool TryGetComponent<T>(this MonoBehaviour mb, out T component) where T : Component
    {
        component = mb.GetComponent<T>();
        return component != null;
    }
    #endregion

    #region GameObject Operations
    /// <summary>
    /// 安全销毁对象
    /// Safely destroy object
    /// </summary>
    public static void SafeDestroy(this MonoBehaviour mb)
    {
        if (Application.isPlaying)
            GameObject.Destroy(mb.gameObject);
        else
            GameObject.DestroyImmediate(mb.gameObject);
    }

    /// <summary>
    /// 设置游戏对象的激活状态
    /// Set GameObject active state
    /// </summary>
    public static void SetActive(this MonoBehaviour mb, bool state)
    {
        if (mb && mb.gameObject)
            mb.gameObject.SetActive(state);
    }
    #endregion

    #region Coroutine Management
    /// <summary>
    /// 停止所有协程并开始新的协程
    /// Stop all coroutines and start a new one
    /// </summary>
    public static Coroutine RestartCoroutine(this MonoBehaviour mb, IEnumerator routine)
    {
        mb.StopAllCoroutines();
        return mb.StartCoroutine(routine);
    }

    /// <summary>
    /// 循环执行直到返回false
    /// Execute repeatedly until returns false
    /// </summary>
    public static Coroutine ExecuteUntil(this MonoBehaviour mb, Func<bool> condition)
    {
        return mb.StartCoroutine(ExecuteUntilCoroutine(condition));
    }

    private static IEnumerator ExecuteUntilCoroutine(Func<bool> condition)
    {
        while (condition())
            yield return null;
    }
    #endregion


    #region Use
    //public class ExampleUsage : MonoBehaviour
    //{
    //    private void Start()
    //    {
    //        // 延迟执行
    //        this.Delay(2f, () => {
    //            Debug.Log("2秒后执行");
    //        });

    //        // 获取或添加组件
    //        var rigidbody = this.GetOrAddComponent<Rigidbody>();

    //        // 尝试获取组件
    //        if (this.TryGetComponent<AudioSource>(out var audioSource))
    //        {
    //            audioSource.Play();
    //        }

    //        // 重置变换
    //        this.ResetTransform();

    //        // 销毁所有子对象
    //        this.DestroyChildren();

    //        // 循环执行直到条件为false
    //        this.ExecuteUntil(() => {
    //            // 返回true继续执行，返回false停止
    //            return transform.position.y > 0;
    //        });
    //    }
    //}
    #endregion

//功能说明(Features)：
//延迟执行(Delay Actions)
//Delay: 普通延迟执行
//DelayRealtime: 真实时间延迟执行
//组件操作(Component Operations)
//GetOrAddComponent: 获取或添加组件
//TryGetComponent: 安全获取组件
//游戏对象操作(GameObject Operations)
//SafeDestroy: 安全销毁对象
//SetActive: 设置激活状态
//协程管理(Coroutine Management)
//RestartCoroutine: 重启协程
//ExecuteUntil: 循环执行直到条件满足
//变换操作(Transform Operations)
//ResetTransform: 重置变换
//DestroyChildren: 销毁所有子对象
//这些扩展方法可以让你的代码更简洁、更易读，并且减少重复代码。根据项目需求，你可以继续添加更多实用的扩展方法。

}