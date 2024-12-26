using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class UniTaskUtils : MonoBehaviour
{
    // 并行执行多个任务
    private async UniTaskVoid ParallelExample()
    {
        // 同时执行多个任务
        await UniTask.WhenAll(
            Task1Async(),
            Task2Async(),
            Task3Async()
        );

        // 等待任意一个任务完成
        await UniTask.WhenAny(
            Task1Async(),
            Task2Async()
        );
    }


    private async UniTask Task1Async()
    {
        await UniTask.Delay(1000);
    }
    private async UniTask Task2Async()
    {
        await UniTask.Delay(2000);
    }
    private async UniTask Task3Async()
    {
        await UniTask.Delay(3000);
    }

    #region unity自带的异步API转换为UniTask
    // 异步加载场景
    private async UniTask LoadSceneAsync()
    {
        await SceneManager.LoadSceneAsync("SceneName").ToUniTask();
    }

    // 异步加载资源
    private async UniTask<GameObject> LoadPrefabAsync()
    {
        var prefab = await Resources.LoadAsync<GameObject>("PrefabName")
            .ToUniTask();
        return prefab as GameObject;
    }
    #endregion

    // 游戏初始化
    private async UniTaskVoid InitializeGame()
    {
        try
        {
            await UniTask.Delay(100);
            // 显示加载界面
            //ShowLoadingUI(true);

            // 并行加载资源
            //var (configTask, audioTask, levelTask) = await UniTask.WhenAll(
            //    LoadConfigAsync(),
            //    LoadAudioAsync(),
            //    LoadLevelAsync()
            //);

            // 初始化系统
            //await InitializeSystemsAsync();

            // 隐藏加载界面
            //ShowLoadingUI(false);

            // 开始游戏
            //StartGame();
        }
        catch (Exception e)
        {
            Debug.LogError($"初始化失败: {e.Message}");
            // 处理错误
        }
    }


    private bool _isGameOver;
    // 异步更新循环
    private async UniTaskVoid GameLoop()
    {
        while (!_isGameOver)
        {
            // 更新游戏逻辑
            await UpdateGameLogicAsync();

            // 等待下一帧
            await UniTask.Yield();  //主线程中的游戏逻辑
            // 等待下一帧,按unity
            //await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            //保持在当前线程中执行
            //await UniTask.DelayFrame(1);  //在线程池中可以循环的游戏逻辑
        }
    }

    private async UniTask UpdateGameLogicAsync()
    {
        await UniTask.Delay(1); // 0.001s
    }
}