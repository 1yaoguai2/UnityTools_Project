using System.Collections.Generic;
using UnityEngine;
using XTools.UI;

/// <summary>
/// 动态UI框架使用示例
/// </summary>
public class AllCanvasManager : MonoBehaviour
{
    void Start()
    {
        InitUIPrefabsPath();
        OpenUIs();
    }

    private void Update()
    {
        InputTestUpdate();
    }
    
    /// <summary>
    /// 初始化UI预制体地址
    /// </summary>
    private void InitUIPrefabsPath()
    {
        Dictionary<string, string> uIPath = new Dictionary<string, string>();
        // uIPath.Add(UIConst.MainCanvas, UIConst.MainCanvas);
        // uIPath.Add(UIConst.ConveyCanvas, UIConst.ConveyCanvas);
        // uIPath.Add(UIConst.CraneCanvas, UIConst.CraneCanvas);
        // uIPath.Add(UIConst.TimeCanvas, UIConst.TimeCanvas);
        //UIManager.Instance.InitDics(uIPath);
    }
    
    /// <summary>
    /// 打开窗口
    /// </summary>
    private void OpenUIs()
    {
        // UIManager.Instance.OpenPanel(UIConst.TimeCanvas);
        // UIManager.Instance.OpenPanel(UIConst.MainCanvas);
    }
    
    /// <summary>
    /// 输入测试窗口
    /// </summary>
    void InputTestUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UIManager.Instance.OpenPanel(UIConst.ConveyCanvas);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            UIManager.Instance.ClosePanel(UIConst.ConveyCanvas);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            UIManager.Instance.OpenPanel(UIConst.CraneCanvas);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            UIManager.Instance.ClosePanel(UIConst.CraneCanvas);
        }
    }
   
}

