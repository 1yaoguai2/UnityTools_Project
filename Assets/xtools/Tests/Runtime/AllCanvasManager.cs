using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using XTools.UI;

/// <summary>
/// 动态UI框架使用示例
/// </summary>
public class AllCanvasManager : MonoBehaviour
{
    public AssetLabelReference uiLabel;
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
    /// 初始化UI资产
    /// </summary>
    private void InitUIPrefabsPath()
    {
        Dictionary<string, string> uIPath = new Dictionary<string, string>();

        //通过标签查找所有资源
        Addressables.LoadAssetsAsync<GameUISO>(uiLabel, (result) =>
        {
              Debug.Log("加载UI" + result.name);
              Debug.Log("UI类型"+ result.uiType);
            
              //uIPath.Add(result.name,result.name);
        });
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

