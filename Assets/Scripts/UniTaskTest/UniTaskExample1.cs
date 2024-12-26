using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// uniTask
/// 基础用法
/// </summary>
public class UniTaskExample1 : MonoBehaviour
{
    void Start()
    {
        CustomLogger.Log($"Star执行,当前帧{Time.frameCount}");
        StartAsync();
        CustomLogger.Log($"Star调用UniTaskVoid后,当前帧{Time.frameCount}");
    }

    private async UniTaskVoid StartAsync()
    {
        await UniTask.Delay(1000);
        CustomLogger.Log($"StarAsync 1S后执行,当前帧{Time.frameCount}");

        await UniTask.Yield();
        CustomLogger.Log($"StarAsync 1帧后执行,当前帧{Time.frameCount}");

        //调用其他异步方法
        int resoult = await DoSomethingAsync();
        CustomLogger.Log($"StarAsync 调用消耗1秒的异步方法,结果{resoult},当前帧{Time.frameCount}");
    }

    private async UniTask<int> DoSomethingAsync()
    {
        await UniTask.Delay(1000);
        return 43;
    }


    /*日志结果
[11:36:35.602] Star执行, 当前帧1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample1.cs
Line: 14
[11:36:35.618] Star调用UniTaskVoid后, 当前帧1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample1.cs
Line: 16
[11:36:36.960] StarAsync 1S后执行,当前帧320
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample1.cs
Line: 22
[11:36:36.963] StarAsync 1帧后执行,当前帧321
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample1.cs
Line: 25
[11:36:37.966] StarAsync 调用消耗1秒的异步方法,结果43,当前帧703
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskExample1.cs
Line: 29

=== Log End ===*/
}
