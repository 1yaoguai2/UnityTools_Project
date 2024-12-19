using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        //LogManager.Log("开始游戏");
        CustomLogger.Log("开始游戏2.0",this);
        CustomLogger.LogWarning("游戏报警2.0");
    }

    void Update()
    {
        //LogManager.LogError("测试报错");
        CustomLogger.LogError("测试报错2.0");
    }
}
