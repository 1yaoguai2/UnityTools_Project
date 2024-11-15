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
        uIPath.Add(UIConst.MainCanvas, UIConst.MainCanvas);
        uIPath.Add(UIConst.ConveyCanvas, UIConst.ConveyCanvas);
        uIPath.Add(UIConst.CraneCanvas, UIConst.CraneCanvas);
        uIPath.Add(UIConst.TimeCanvas, UIConst.TimeCanvas);
        UIManager.Instance.InitDics(uIPath);
    }
    
    /// <summary>
    /// 打开窗口
    /// </summary>
    private void OpenUIs()
    {
        UIManager.Instance.OpenPanel(UIConst.TimeCanvas);
        UIManager.Instance.OpenPanel(UIConst.MainCanvas);
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
//Assets/Resources/Prefabs/UI文件夹下有下面几个UI预制体
//预制体要求，自带Canvas，常用Canvas继承BaseScalePanel或者BasePellucidityPanel（清晰度）
//不常用Canvas继承BasePanel，关闭的时候直接删除
class UIConst
{
    public const string MainCanvas = "MainCanvas";
    public const string ConveyCanvas = "ConveyCanvas";
    public const string CraneCanvas = "CraneCanvas";
    public const string TimeCanvas = "TimeCanvas";
}