using System.Collections.Generic;
using UnityEngine;
using XTools.UI;

/// <summary>
/// 动态UI框架使用示例
/// </summary>
public class AllCanvasController : MonoBehaviour
{
    //该场景下所有UI名称
    public List<string> allUINames = new List<string>();
    //该场景需要初始化启动的ui名称
    public List<string> initUINames = new List<string>();

    //菜单窗口名称
    public string sceneMenuName;
    //当esc按下时，会优先关闭这些界面
    public List<string> levelPanelNames = new List<string>();
    void Awake()
    {
        InitUIPrefabsPath();
        UIInit();
    }

    private void Update()
    {
        InputTestUpdate();
        PanelOpen();
    }

    /// <summary>
    /// 初始化UI预制体地址
    /// </summary>
    private void InitUIPrefabsPath()
    {
        Dictionary<string, string> uIPath = new Dictionary<string, string>();
        foreach (var uiName in allUINames)
        {
            uIPath.Add(uiName, uiName);
        }
        // uIPath.Add(UIConst.TimeCanvas, UIConst.TimeCanvas);
        UIManager.Instance.InitDics(uIPath);
    }

    /// <summary>
    /// 初始化生成窗口
    /// </summary>
    private void UIInit()
    {
        // UIManager.Instance.OpenPanel(UIConst.MapCanvas);
        // UIManager.Instance.OpenPanel(UIConst.MainCanvas);
        // UIManager.Instance.OpenPanel(UIConst.ConfirmCanvas);
        foreach (var uiName in initUINames)
        {
            UIManager.Instance.OpenPanel(uiName);
        }
    }

    /// <summary>
    /// 窗口打开
    /// </summary>
    private void PanelOpen()
    {
        EscapeDownOpenOrClosePanel();
    }

    /// <summary>
    /// Escape按键的一些功能
    /// </summary>
    private void EscapeDownOpenOrClosePanel()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isCloseLevelPanel = false;
            foreach (var planeName in levelPanelNames)
            {
                if (UIManager.Instance.panelDic.TryGetValue(planeName, out BasePanel levelPanel))
                {
                    if (levelPanel.isShow)
                    {
                        UIManager.Instance.ClosePanel(planeName);
                        isCloseLevelPanel = true;
                    }
                }
            }
            if (isCloseLevelPanel) return;
            if (string.IsNullOrEmpty(sceneMenuName)) return;
            if (UIManager.Instance.panelDic.TryGetValue(sceneMenuName, out BasePanel menuPanel))
            {
                if (menuPanel.isShow)
                {
                    UIManager.Instance.ClosePanel(sceneMenuName);
                    //var component = Camera.main.GetComponent<FirstPersonController>();
                    //if (component is not null)
                    //{
                    //    component.LockCursor = false;
                    //}
                    return;
                }
            }

            UIManager.Instance.OpenPanel(sceneMenuName);
            //var firstPersonController = Camera.main.GetComponent<FirstPersonController>();
            //if (firstPersonController is not null)
            //{
            //    firstPersonController.LockCursor = true;
            //}

            //没有菜单界面，直接退出
            if (sceneMenuName == "ConfirmCanvas")
            {
                UIManager.Instance.panelDic.TryGetValue(UIConst.ConfirmCanvas, out BasePanel confirmPanel);
                //var confirmCanvas = confirmPanel as ConfirmCanvasController;
                //if (confirmCanvas != null) confirmCanvas.ShowWindow("是否关闭软件！", Exit, () =>
                //{
                //    if (firstPersonController is not null)
                //    {
                //        firstPersonController.LockCursor = false;
                //    }
                //});
            }
        }
    }

    /// <summary>
    /// 按键控制的界面
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="planeName"></param>
    private void KeyDownOpenPanel(KeyCode keyCode, string planeName)
    {
        if (Input.GetKeyDown(keyCode))
        {
            if (UIManager.Instance.panelDic.TryGetValue(planeName, out BasePanel taskPanel))
            {
                if (taskPanel.isShow)
                {
                    UIManager.Instance.ClosePanel(planeName);
                    return;
                }
            }

            //未升成panel或者窗口关闭
            UIManager.Instance.OpenPanel(planeName);
        }
    }

    /// <summary>
    /// 输入测试窗口
    /// </summary>
    void InputTestUpdate()
    {
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     UIManager.Instance.OpenPanel(UIConst.ConveyCanvas);
        // }
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     UIManager.Instance.ClosePanel(UIConst.ConveyCanvas);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     UIManager.Instance.OpenPanel(UIConst.CraneCanvas);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     UIManager.Instance.ClosePanel(UIConst.CraneCanvas);
        // }
    }

    public static void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

