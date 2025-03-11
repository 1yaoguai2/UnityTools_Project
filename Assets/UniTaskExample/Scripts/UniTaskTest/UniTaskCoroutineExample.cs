using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// UniTask 和 协程对比
/// </summary>
public class UniTaskCoroutineExample : MonoBehaviour
{
    #region 基础等待
    // 协程版本
    private IEnumerator CoroutineWait()
    {
        Debug.Log("开始等待");
        yield return new WaitForSeconds(2f);
        Debug.Log("等待结束");
    }

    // UniTask版本
    private async UniTaskVoid UniTaskWait()
    {
        Debug.Log("开始等待");
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        Debug.Log("等待结束");
    }
    #endregion

    #region 循环执行
    // 协程版本
    private IEnumerator CoroutineLoop()
    {
        while (true)
        {
            Debug.Log("循环执行");
            yield return new WaitForSeconds(1f);
        }
    }

    CancellationTokenSource cts = new CancellationTokenSource();
    // UniTask版本
    private async UniTaskVoid UniTaskLoop(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Debug.Log("循环执行");
            await UniTask.Delay(1000, cancellationToken: cancellationToken);
        }
    }
    #endregion

    #region 渐变效果
    // 协程版本
    private IEnumerator CoroutineFade()
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
            yield return null;
        }
    }

    // UniTask版本
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
    #endregion

    #region 等待条件满足
    // 协程版本
    private IEnumerator CoroutineWaitUntil()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Debug.Log("按下了空格键");
    }

    // UniTask版本
    private async UniTaskVoid UniTaskWaitUntil(CancellationToken cancellationToken = default)
    {
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space),
            cancellationToken: cancellationToken);
        Debug.Log("按下了空格键");
    }
    #endregion

    #region 加载资源
    // 协程版本
    private IEnumerator CoroutineLoadResource()
    {
        var request = Resources.LoadAsync<GameObject>("Prefab");
        yield return request;
        if (request.asset != null)
        {
            Instantiate(request.asset);
        }
    }

    // UniTask版本
    private async UniTaskVoid UniTaskLoadResource()
    {
        var prefab = await Resources.LoadAsync<GameObject>("Prefab").ToUniTask();
        if (prefab != null)
        {
            Instantiate(prefab);
        }
    }
    #endregion

    #region 序列动作
    // 协程版本
    private IEnumerator CoroutineSequence()
    {
        Debug.Log("第一步");
        yield return new WaitForSeconds(1f);

        Debug.Log("第二步");
        yield return new WaitForSeconds(1f);

        Debug.Log("第三步");
    }

    // UniTask版本
    private async UniTaskVoid UniTaskSequence(CancellationToken cancellationToken = default)
    {
        Debug.Log("第一步");
        await UniTask.Delay(1000, cancellationToken: cancellationToken);

        Debug.Log("第二步");
        await UniTask.Delay(1000, cancellationToken: cancellationToken);

        Debug.Log("第三步");
    }
    #endregion

    #region 并行执行
    // 协程版本需要手动管理多个协程

    // UniTask版本
    private async UniTaskVoid UniTaskParallel(CancellationToken cancellationToken = default)
    {
        // 同时执行多个任务
        await UniTask.WhenAll(
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

    private async UniTask Task2(CancellationToken cancellationToken)
    {
        await UniTask.Delay(2000, cancellationToken: cancellationToken);
        Debug.Log("Task 2 完成");
    }

    private async UniTask Task3(CancellationToken cancellationToken)
    {
        await UniTask.Delay(3000, cancellationToken: cancellationToken);
        Debug.Log("Task 3 完成");
    }
    #endregion

    #region 动画等待
    // 协程版本
    private IEnumerator CoroutineWaitAnimation()
    {
        var animator = GetComponent<Animator>();
        animator.Play("AnimationName");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        Debug.Log("动画播放完成");
    }

    // UniTask版本
    private async UniTaskVoid UniTaskWaitAnimation(CancellationToken cancellationToken = default)
    {
        var animator = GetComponent<Animator>();
        animator.Play("AnimationName");
        await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length),
            cancellationToken: cancellationToken);
        Debug.Log("动画播放完成");
    }
    #endregion

    /// <summary>
    /// 扩展方法，超时警告
    /// </summary>
    /// <returns></returns>
    private async UniTask Monthed()
    {
        //超时警告
        await UniTask.Delay(1000).Timeout(TimeSpan.FromSeconds(2));
    }
   
    /// <summary>
    /// 取消所有正在执行的UniTask, 这些Unitask必须可被取消
    /// </summary>
    private void CanecleAllUniTask()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        // 取消所有正在进行的UniTask，且这些Unitask,必须传入cts参数
        cts?.Cancel();
        cts?.Dispose();

        //关闭所有正在执行的协程
        StopAllCoroutines();
    }


    #region 协程内部调用UniTask

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
            //跟新UI
            _idText.text = index.ToString();
            yield return new WaitForSeconds(10f);
        }
    }
    

    private async UniTask<int> GetIntAsync()
    {
        await UniTask.DelayFrame(10);  //延迟10帧
        await UniTask.Yield();  //一帧后切回主线程
        return 10;
    }

    #endregion
}