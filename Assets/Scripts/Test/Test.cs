using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LogManager.Log("��ʼ��Ϸ");
    }

    // Update is called once per frame
    void Update()
    {
        LogManager.LogError("���Ա���");
    }
}
