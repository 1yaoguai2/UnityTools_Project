using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using System.Threading;

/// <summary>
/// 更复杂的线程切换示例，包含错误处理和取消操作：
/// </summary>
public class AdvancedThreadSwitchExample : MonoBehaviour
{
    private CancellationTokenSource _cts;

    private void Start()
    {
        StartAsync();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //满足某种条件取消操作
            Cancel();
        }
    }

    private async UniTaskVoid StartAsync()
    {
        _cts = new CancellationTokenSource();

        try
        {
            await ProcessDataAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            CustomLogger.Log("操作被取消");
            _cts?.Dispose();
        }
        catch (Exception ex)
        {
            CustomLogger.LogError($"发生错误: {ex.Message}");
        }
    }

    private async UniTask ProcessDataAsync(CancellationToken cancellationToken = default)
    {
        // 在主线程准备数据
        var data = PrepareData();

        // 切换到线程池处理数据
        await UniTask.SwitchToThreadPool();

        try
        {
            var processedData = await ProcessDataInBackground(data, cancellationToken);

            // 切回主线程更新UI
            await UniTask.SwitchToMainThread(PlayerLoopTiming.Update);
            UpdateUI(processedData);

            // 再次切换到线程池做后续处理
            await UniTask.SwitchToThreadPool();
            await SaveDataAsync(processedData, cancellationToken);

            // 最后切回主线程完成操作
            await UniTask.SwitchToMainThread();
            CompleteOperation();
        }
        catch (Exception)
        {
            // 确保异常处理在主线程
            await UniTask.SwitchToMainThread();
            throw;
        }
    }

    private int[] PrepareData()
    {
        CustomLogger.Log($"准备数据 在主线程: {Thread.CurrentThread.ManagedThreadId}");
        return new int[1000];
    }

    private async UniTask<float[]> ProcessDataInBackground(int[] data, CancellationToken cancellationToken)
    {
        CustomLogger.Log($"处理数据 在线程: {Thread.CurrentThread.ManagedThreadId}");

        float[] results = new float[data.Length];

        // 模拟耗时处理
        for (int i = 0; i < data.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            results[i] = Mathf.Sqrt(i);

            if (i % 200 == 0)
            {
                await UniTask.Delay(1, cancellationToken: cancellationToken);
            }
        }

        return results;
    }

    private void UpdateUI(float[] processedData)
    {
        CustomLogger.Log($"更新UI 在主线程: {Thread.CurrentThread.ManagedThreadId}");
        // 更新UI操作
    }

    private async UniTask SaveDataAsync(float[] data, CancellationToken cancellationToken)
    {
        CustomLogger.Log($"保存数据 在线程: {Thread.CurrentThread.ManagedThreadId}");
        await UniTask.Delay(1000, cancellationToken: cancellationToken);
        // 保存数据操作,耗时10秒
        await UniTask.Delay(10000, cancellationToken: cancellationToken);
    }

    private void CompleteOperation()
    {
        CustomLogger.Log($"完成操作 在主线程: {Thread.CurrentThread.ManagedThreadId}");
    }

    private void Cancel()
    {
        _cts?.Cancel();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    /*
[15:22:31.186] 准备数据 在主线程: 1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\AdvancedThreadSwitchExample.cs
Line: 79
[15:22:31.268] 处理数据 在线程: 258474
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\AdvancedThreadSwitchExample.cs
Line: 85
[15:22:32.181] 更新UI 在主线程: 1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\AdvancedThreadSwitchExample.cs
Line: 107
[15:22:32.182] 保存数据 在线程: 258474
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\AdvancedThreadSwitchExample.cs
Line: 113
//取消操作
[15:22:36.241] 操作被取消
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\AdvancedThreadSwitchExample.cs
Line: 37
//不取消操作
[15:25:08.089] 完成操作 在主线程: 1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\AdvancedThreadSwitchExample.cs
Line: 111
     */
}