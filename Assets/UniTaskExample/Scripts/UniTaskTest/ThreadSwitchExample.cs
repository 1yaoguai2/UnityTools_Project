using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;


/// <summary>
/// 公若不弃，布愿拜为衣服
/// 反复横跳
/// Time.frameCount 只认主线程为义父
/// </summary>
public class ThreadSwitchExample : MonoBehaviour
{
    private void Start()
    {
        StartAsync();
    }
    private async UniTaskVoid StartAsync()
    {
        CustomLogger.Log($"开始 在主线程: {Thread.CurrentThread.ManagedThreadId},当前帧{Time.frameCount}");

        // 切换到线程池
        await UniTask.SwitchToThreadPool();
        CustomLogger.Log($"在线程池中: {Thread.CurrentThread.ManagedThreadId}");

        // 执行耗时操作
        await HeavyCalculation();

        // 切回主线程
        await UniTask.SwitchToMainThread();
        CustomLogger.Log($"回到主线程: {Thread.CurrentThread.ManagedThreadId},当前帧{Time.frameCount}");
        DoSomethingInMainThread();

        // 再次切回线程池
        await UniTask.SwitchToThreadPool();
        CustomLogger.Log($"又回到线程池: {Thread.CurrentThread.ManagedThreadId}");
    }

    // 在主线程执行的操作
    private void DoSomethingInMainThread()
    {
        // Unity API 调用必须在主线程
        transform.position = Vector3.one;
        CustomLogger.Log($"必须回到主线程执行Unity操作,当前帧{Time.frameCount}");
    }

    // 模拟耗时计算
    private async UniTask HeavyCalculation()
    {
        // 模拟耗时操作
        await UniTask.Delay(1000);

        // 执行一些计算
        long result = 0;
        for (int i = 0; i < 1000000; i++)
        {
            result += i;
        }

        CustomLogger.Log($"计算完成: {result}");
    }

    /*
[14:59:43.176] 开始 在主线程: 1,当前帧1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\ThreadSwitchExample.cs
Line: 19
[14:59:43.255] 在线程池中: 258474
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\ThreadSwitchExample.cs
Line: 23
[14:59:45.025] 计算完成: 499999500000
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\ThreadSwitchExample.cs
Line: 59
[14:59:45.029] 回到主线程: 1,当前帧301
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\ThreadSwitchExample.cs
Line: 30
[14:59:45.029] 必须回到主线程执行Unity操作,当前帧301
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\ThreadSwitchExample.cs
Line: 43
[14:59:45.030] 又回到线程池: 258474
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\ThreadSwitchExample.cs
Line: 35
     */
}