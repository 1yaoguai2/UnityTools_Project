using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncWaitMove : MonoBehaviour
{
    //void Start()
    //{
    //    ////1.
    //    //AsyncMove();

    //}

    private async void Start()
    {
        CustomLogger.Log($"异步场景开始！{Time.frameCount}");
        Move move = new Move();
        await move.AsyncMove(transform);
        CustomLogger.Log($"第一次移动完成！{Time.frameCount}");
        CustomLogger.Log($"开始另一次移动{Time.frameCount}");
        await AsyncMove();
        CustomLogger.Log($"另一次移动完成！{Time.frameCount}");
    }

    /// <summary>
    /// 1.不依赖于Mono
    /// </summary>
    async Task AsyncMove()
    {
        await Task.Delay(TimeSpan.FromSeconds(1f));
        transform.position += new Vector3(2,0,0);
    }
}

/// <summary>
/// 2.定义在外部类中
/// </summary>
public class Move
{
    public async Task AsyncMove(Transform trans)
    {
        await Task.Delay(TimeSpan.FromSeconds(1f));
        trans.position += new Vector3(2,0,0);
    }
}
