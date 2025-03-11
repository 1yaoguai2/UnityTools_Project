# UniTask介绍

### 1.主要优势

```
性能优化
零内存分配
更低的GC压力
更好的性能表现
与Unity生命周期完美配合
支持协程转换
支持DOTween等插件
取消操作
超时控制
异步枚举
并行执行
错误处理
更好的异常处理
追踪调用
调试友好
```

### 2.Unity使用

```
 // 通过 Unity Package Manager 安装:
 // 1. 打开 Package Manager
 // 2. 点击 "+" 按钮
 // 3. 选择 "Add package from git URL"
 // 4. 输入: https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```



# 用例展示

### 1.延迟操作

```C#
private async UniTaskVoid StartAsync()
    {
        await UniTask.Delay(1000);  //延迟1000ms
        await UniTask.Delay(TimeSpan.FromSeconds(1));  //延迟1s
        await UniTask.Delay(1000,delayTiming:PlayerLoopTiming.FixedUpdate);  //以unity的fixedUpdate时间来延迟1s

        await UniTask.DelayFrame(3);  //延迟3帧
        await UniTask.DelayFrame(3,PlayerLoopTiming.FixedUpdate);  //延迟3帧(按照FixedUpdate)


        await UniTask.Yield();  //延迟Update的一帧，无论在不在主线程都会延迟一帧，切回主线程。
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);  //延迟1帧(按照FixedUpdate)

        await UniTask.SwitchToMainThread(); //等待一帧切换主线程跑,如果已经在主线程，则不会有延迟，直接继续执行

    }
```



### 2.协程内调用

```c#
    //配合协程循环跟新UI
    private Text _idText;
    private IEnumerator UpdateUiText()
    {
        int index = 0;
        while (true)
        {
            var task = GetIntAsync();
            yield return task.ToCoroutine(result =>
            {
                index = result;
            });
            //在主线程跟新UI
            _idText.text = index.ToString();
            yield return new WaitForSeconds(10f);
        }
    }
    

    private async UniTask<int> GetIntAsync()
    {
        await UniTask.DelayFrame(10);  //延迟10帧
        await UniTask.Yield();  //延迟一帧后切回主线程
        return 10;
    }


```

### 3.UniTask取消令牌

```C#
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Example : MonoBehaviour
{
    private CancellationTokenSource cts;

    async void Start()
    {
        cts = new CancellationTokenSource();
        try
        {
            await MyCoroutine(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Coroutine was canceled!");
        }
        catch (Exception ex)
        {
            CustomLogger.LogError($"发生错误: {ex.Message}");
        }
    }

    async UniTask MyCoroutine(CancellationToken token)
    {
        Debug.Log("Coroutine started!");
        await UniTask.Delay(2000, cancellationToken: token); // 等待2秒，并检查取消令牌
        Debug.Log("2 seconds passed!");
    }
    
    void OnDisable()
    {
       cts?.Cancel(); // 在对象隐藏时取消协程
    }

    void OnDestroy()
    {
        cts?.Cancel(); // 在对象销毁时取消协程
    }
}
```

### 4.UniTask 基础用法

```C#
//条件等待 
private async UniTaskVoid UniTaskWaitUntil(CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space),
            cancellationToken: cancellationToken);
        Debug.Log("按下了空格键");
    }

//颜色渐变
 private async UniTaskVoid UniTaskFade(CancellationToken cancellationToken = default)
    {
        float duration = 1f;
        float elapsed = 0f;
        var material = GetComponent<Renderer>().material;
        Color startColor = material.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            material.color = Color.Lerp(startColor, endColor, t);
            await UniTask.Yield(cancellationToken);
        }
    }

//资源加载
 private async UniTaskVoid UniTaskLoadResource()
    {
        var prefab = await Resources.LoadAsync<GameObject>("Prefab").ToUniTask();
        if (prefab != null)
        {
            Instantiate(prefab);
        }
    }

//顺序执行
 private async UniTaskVoid UniTaskSequence(CancellationToken cancellationToken = default)
    {
        Debug.Log("第一步");
        await UniTask.Delay(1000, cancellationToken: cancellationToken);

        Debug.Log("第二步");
        await UniTask.Delay(1000, cancellationToken: cancellationToken);

        Debug.Log("第三步");
    }

//并行执行并等待全部完成
 private async UniTaskVoid UniTaskParallel(CancellationToken cancellationToken = default)
    {
        // 同时执行多个任务
        await UniTask.WhenAll(
            Task1(cancellationToken),
            Task2(cancellationToken),
            Task3(cancellationToken)
        );
      // 等待任意一个任务完成
        await UniTask.WhenAny(
            Task1(cancellationToken),
            Task2(cancellationToken),
            Task3(cancellationToken)
        );
    }

    private async UniTask Task1(CancellationToken cancellationToken)
    {
        await UniTask.Delay(1000, cancellationToken: cancellationToken);
        Debug.Log("Task 1 完成");
    }

//配合动画播放，并在播放完成后执行相关逻辑
private async UniTaskVoid UniTaskWaitAnimation(CancellationToken cancellationToken = default)
    {
        var animator = GetComponent<Animator>();
        animator.Play("AnimationName");
        await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length),
            cancellationToken: cancellationToken);
        Debug.Log("动画播放完成");
    }

```

### 5.UniTask 超时报警

```C#
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Example : MonoBehaviour
{
    async void Start()
    {
        try
        {
            await MyCoroutine().Timeout(2000); // 设置超时时间为2秒
            Debug.Log("Coroutine finished successfully!");
        }
        catch (TimeoutException)
        {
            Debug.LogWarning("Coroutine timed out!");
        }
    }

    //一个耗时未知的UniTask方法
    async UniTask MyCoroutine()
    {
        Debug.Log("Coroutine started!");
        await UniTask.Delay(3000); // 模拟一个耗时3秒的操作
        Debug.Log("Coroutine finished!");
    }
}
```

### 6.线程切换

```
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
        await UniTask.SwitchToMainThread(); //或者使用Yiled()，区别请看1
        CustomLogger.Log($"回到主线程: {Thread.CurrentThread.ManagedThreadId},当前帧{Time.frameCount}");
        DoSomethingInMainThread();

        // 再次切回线程池
        await UniTask.SwitchToThreadPool();
        CustomLogger.Log($"又回到线程池: {Thread.CurrentThread.ManagedThreadId}");
        await UniTask.SwitchToThreadPool();
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

}
```

