using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using XTools.UI;

/// <summary>
/// 动态UI框架使用示例
/// 使用步骤，
/// 1.打开AddressableGroup窗口初始化，
/// 2.将ConfirmUI的So文件拖拽到Group窗口，并添加label
/// 3.将ConfirmGUI的Prefab文件拖拽到Group窗口新建的动态资源UI下
/// 4.生成空物体AllCanvas
/// 5.生成空物体挂载AllCanvasController，选择AssetsLabel
/// 6.运行，按下Esc，正常打开ConfirmGUI窗口即可
/// 7.所有Canvas都采用长款0.5比例的缩放，所有Canvas控制脚本都继承Basepanel
/// 8.所有窗口预制体都有对应的GameUISO文件，并成为Addressable，对应的GameUiSO也成为Addressable
/// </summary>
public class TestConfirmWindowGUI : MonoBehaviour
{
    public AssetReference confirmGUI;
    private string confirmGuiName ="ConfirmGUI";
    public bool isTest1;
    public bool isTest2;

    IEnumerator Start()
    {
        Dictionary<string, GameUISO> uIPath = new Dictionary<string, GameUISO>();
        var handle = confirmGUI.LoadAssetAsync<GameUISO>();
        while (!handle.IsDone)
        {
            yield return null;
        } 
        uIPath.Add(handle.Result.name, handle.Result);
        UIManager.Instance.InitDics(uIPath);
        Addressables.Release(handle);
    }

    void Update()
    {
        if (isTest1)
        {
            isTest1 = false;
            UIManager.Instance.OpenPanel(confirmGuiName);
            if (UIManager.Instance.openPanelDic.TryGetValue(confirmGuiName, out BasePanel confirmWindow))
            {
                ConfirmWindowGUI newConfirmWindow = confirmWindow as ConfirmWindowGUI;
                newConfirmWindow.LoadConfirmWindowGUI("退出程序",Exit,null);
                newConfirmWindow.OpenPanel(confirmGuiName);
            }
        }

        if (isTest2)
        {
            isTest2 = false;
            UIManager.Instance.OpenPanel(confirmGuiName);
            if (UIManager.Instance.openPanelDic.TryGetValue(confirmGuiName, out BasePanel confirmWindow))
            {
                ConfirmWindowGUI newConfirmWindow = confirmWindow as ConfirmWindowGUI;
                newConfirmWindow.LoadConfirmWindowGUI("退出程序",null,null);
                newConfirmWindow.OpenPanel(confirmGuiName);
            }
        }
    }

    private void Exit()
    {
        Debug.Log("假装退出");
    }
}