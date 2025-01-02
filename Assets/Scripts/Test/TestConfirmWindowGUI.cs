using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XTools.UI;

public class TestConfirmWindowGUI : MonoBehaviour
{
    public bool isTest1;
    public bool isTest2;
    private readonly string confirmWindowGUI = "ConfirmWindowGUI";

    void Start()
    {
        Dictionary<string, string> uIPath = new Dictionary<string, string>();

        uIPath.Add(confirmWindowGUI, confirmWindowGUI);
        UIManager.Instance.InitDics(uIPath);
    }

    void Update()
    {
        if (isTest1)
        {
            isTest1 = false;
            UIManager.Instance.OpenPanel(confirmWindowGUI);
            bool find = UIManager.Instance.panelDic.TryGetValue(confirmWindowGUI, out BasePanel confirmWindow);
            if (find)
            {
                ConfirmWindowGUI newConfirmWindow = confirmWindow as ConfirmWindowGUI;
                newConfirmWindow.LoadConfirmWindowGUI("退出程序",Exit,null);
                newConfirmWindow.OpenPanel(confirmWindowGUI);
            }
        }

        if (isTest2)
        {
            isTest2 = false;
            UIManager.Instance.OpenPanel(confirmWindowGUI);
            bool find = UIManager.Instance.panelDic.TryGetValue(confirmWindowGUI, out BasePanel confirmWindow);
            if (find)
            {
                ConfirmWindowGUI newConfirmWindow = confirmWindow as ConfirmWindowGUI;
                newConfirmWindow.LoadConfirmWindowGUI("退出程序",null,null);
                newConfirmWindow.OpenPanel(confirmWindowGUI);
            }
        }
    }

    private void Exit()
    {
        Debug.Log("假装退出");
    }
}