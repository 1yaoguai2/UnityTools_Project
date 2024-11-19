using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LogManager.Log("开始游戏");
    }

    // Update is called once per frame
    void Update()
    {
        LogManager.LogError("测试报错");
    }
}
