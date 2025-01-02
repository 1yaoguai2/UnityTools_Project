using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLog : MonoBehaviour
{
    void Start()
    {
        CustomLogger.Log("开始游戏");
    }

    void Update()
    {
        CustomLogger.LogError("游戏故障");
    }
}
