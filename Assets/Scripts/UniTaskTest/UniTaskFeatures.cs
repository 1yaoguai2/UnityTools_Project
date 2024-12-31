using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public class UniTaskFeatures : MonoBehaviour
{
    // 取消令牌
    private CancellationTokenSource _cts;
    private void Start()
    {
        Example();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //满足一定条件，取消操作
            CancelOperation();
        }
    }

    private async UniTaskVoid Example()
    {
        _cts = new CancellationTokenSource();
        CustomLogger.Log($"Example开始,当前线程ID：{Thread.CurrentThread.ManagedThreadId}");
        try
        {
            // 带超时的操作
            //await DoLongTaskAsync().TimeoutWithoutException(TimeSpan.FromSeconds(5000)); // 5秒超时
            CustomLogger.Log($"Example可取消操作开始后");
            // 可取消的操作
            await UniTask.Delay(5000, cancellationToken: _cts.Token);

            CustomLogger.Log($"Example等待按下键盘Space开始");
            // 等待条件满足，为true则继续执行，否则等待
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            CustomLogger.Log($"Example等待按下键盘Space成功");
            CustomLogger.Log($"Example等待条件为false开始");
            // 等待条件满足，为false则继续执行，否则等待
            await UniTask.WaitWhile(() => !transform.gameObject.activeInHierarchy);

            //监听值的变化
            CustomLogger.Log($"Example等待transform的世界坐标发生变化");
            var str = await UniTask.WaitUntilValueChanged(this.transform, x => x.position);//第一个参数时判断目标，第二个参数是判断方法的委托。如果这个返回值变的话，即为发生变化。
            CustomLogger.Log($"Example等待transform的世界坐标发生变化成功！{str}");

            // 等待动画完成
            //await transform.DOMove(Vector3.up, 1f).ToUniTask();  //需要插件
            await UniTask.SwitchToThreadPool();
            CustomLogger.Log($"Example进入无限循环方法,当前线程ID：{Thread.CurrentThread.ManagedThreadId}");
            await WhileTask(_cts.Token);

        }
        catch (OperationCanceledException)
        {
            CustomLogger.Log("操作被取消");
        }
        catch (Exception ex)
        {
            CustomLogger.LogError($"发生错误: {ex.Message}");
        }
    }

    private async UniTask DoLongTaskAsync()
    {
        //例如,读取接口数据，读取超时
        int i = UnityEngine.Random.Range(2, 6);
        await UniTask.Delay(i * 1000);
    }

    private async UniTask WhileTask(CancellationToken ctsToken)
    {
        try
        {
            // 周期性执行
            //await foreach (var _ in UniTaskAsyncEnumerable.Interval(TimeSpan.FromSeconds(1000)))
            //{
            //    CustomLogger.Log("每秒执行一次的无限循环");
            //}
            while (true)
            {
                CustomLogger.Log("每秒执行一次的无限循环");
                await UniTask.Delay(1000, cancellationToken:ctsToken);
            }
        }
        catch (Exception)
        {
            await UniTask.SwitchToMainThread();
            CustomLogger.Log($"WhileTask取消操作，当前线程ID:{Thread.CurrentThread.ManagedThreadId}");
            throw;
        }

    }

    // 取消操作
    private void CancelOperation()
    {
        _cts?.Cancel();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    /*
[16:29:25.619] Example开始,当前线程ID：1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 29
[16:29:25.624] Example可取消操作开始后
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 34
[16:29:30.971] Example等待按下键盘Space开始
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 38
[16:29:34.638] Example等待按下键盘Space成功
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 41
[16:29:34.638] Example等待条件为false开始
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 42
[16:29:34.643] Example等待transform的世界坐标发生变化
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 47
[16:29:39.616] Example等待transform的世界坐标发生变化成功！(1.00, 0.00, 0.00)
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 49
[16:29:39.617] Example进入无限循环方法,当前线程ID：651103
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 54
[16:29:39.617] 每秒执行一次的无限循环
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 86
[16:29:40.616] 每秒执行一次的无限循环
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 86
[16:29:41.617] 每秒执行一次的无限循环
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 86
[16:29:42.617] 每秒执行一次的无限循环
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 86
[16:29:43.620] 每秒执行一次的无限循环
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 86
[16:29:44.619] 每秒执行一次的无限循环
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 86
[16:29:44.881] WhileTask取消操作，当前线程ID:1
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 93
[16:29:44.883] 操作被取消
File: D:\XKZ\UnityTools_Project\Assets\Scripts\UniTaskTest\UniTaskFeatures.cs
Line: 60
     */
}