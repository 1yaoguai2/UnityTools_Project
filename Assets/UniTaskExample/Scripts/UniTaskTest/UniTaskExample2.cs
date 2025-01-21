using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// uniTask
/// UniTask.Delay
/// UniTask.DealyFrame
/// UniTask.Yield()
/// </summary>
public class UniTaskExample2 : MonoBehaviour
{
    void Start()
    {
        CustomLogger.Log($"Star执行,当前帧{Time.frameCount}");
        StartAsync();
        CustomLogger.Log($"Star调用UniTaskVoid后,当前帧{Time.frameCount}");
    }

    private async UniTaskVoid StartAsync()
    {
        await UniTask.Delay(1000);  //延迟1000ms
        await UniTask.Delay(TimeSpan.FromSeconds(1));  //延迟1s
        await UniTask.Delay(1000,delayTiming:PlayerLoopTiming.FixedUpdate);  //以unity的fixedUpdate时间来延迟1s
        CustomLogger.Log($"StarAsync 使用Delay延迟 3S后执行,当前帧{Time.frameCount}");

        await UniTask.DelayFrame(3);  //延迟3帧
        await UniTask.DelayFrame(3,PlayerLoopTiming.FixedUpdate);  //延迟3帧(按照FixedUpdate)
        CustomLogger.Log($"StarAsync 使用DelayFrame延迟 6帧后执行，线程{Thread.CurrentThread.Name},当前帧{Time.frameCount}");

        await UniTask.Yield();  //延迟Update的一帧，无论在不在主线程都会延迟一帧，切回主线程。
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);  //延迟1帧(按照FixedUpdate)
        CustomLogger.Log($"StarAsync 使用Yield延迟 2帧后回到主线程执行{Thread.CurrentThread.Name},当前帧{Time.frameCount}");

        await UniTask.SwitchToMainThread(); //等待一帧切换主线程跑,如果已经在主线程，则不会有延迟，直接继续执行
        CustomLogger.Log($"StarAsync 使用SwitchToMainThread延迟 1帧后回到主线程执行{Thread.CurrentThread.Name},当前帧{Time.frameCount}");

    }




    /*日志结果
    
[14:33:53.146] Star执行,当前帧1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample2.cs
Line: 16
[14:33:53.246] Star调用UniTaskVoid后,当前帧1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample2.cs
Line: 18
[14:33:57.389] StarAsync 使用Delay延迟 3S后执行,当前帧1279
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample2.cs
Line: 26
[14:33:57.449] StarAsync 使用DelayFrame延迟 6帧后执行，线程,当前帧1304
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample2.cs
Line: 30
[14:33:57.470] StarAsync 使用Yield延迟 2帧后回到主线程执行,当前帧1311
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample2.cs
Line: 34
[14:33:57.473] StarAsync 使用SwitchToMainThread延迟 1帧后回到主线程执行,当前帧1311   因为已经在主线程上SwitchToMainThread不等待一帧，与Unitask.Yield的区别
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample2.cs
Line: 37

     */
}
