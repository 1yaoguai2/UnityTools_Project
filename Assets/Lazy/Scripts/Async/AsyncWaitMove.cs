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
        CustomLogger.Log("异步场景开始！");
        Move move = new Move();
        await move.AsyncMove(transform);
        CustomLogger.Log("等待移动完成！");
        CustomLogger.Log("另一次移动");
        await AsyncMove();
        CustomLogger.Log("等待另一次移动完成！");
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
