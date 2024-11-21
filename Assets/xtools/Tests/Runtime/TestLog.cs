using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLog : MonoBehaviour
{
    void Start()
    {
        LogManager.Log("开始游戏");
    }

    void Update()
    {
        LogManager.LogError("游戏故障");
    }
}
