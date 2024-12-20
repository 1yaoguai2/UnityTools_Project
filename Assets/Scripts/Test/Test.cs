using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        LogManager.Log("开始游戏");
        CustomLogger.Log("开始游戏2.0",this);
    }

    void Update()
    {
        //CustomLogger.LogWarning("游戏报警2.0");
        //CustomLogger.LogError("测试报错");
        LogManager.LogError("测试报错");
    }
}
