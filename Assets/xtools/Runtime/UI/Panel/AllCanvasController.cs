using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using xTools;
using XTools.UI;

/// <summary>
/// 动态UI框架使用示例
/// </summary>
public class AllCanvasController : MonoBehaviour
{
    //菜单窗口名称
    private string _sceneMenuName;

    //当esc按下时，会优先关闭这些界面
    private List<string> windowPanelNames;

    public AssetLabelReference assetsLabel;

    void Awake()
    {
        StartCoroutine(InitUI());
    }

    private void Update()
    {
        InputTestUpdate();
        PanelOpen();
    }

    /// <summary>
    /// 初始化UI预制体地址
    /// </summary>
    private IEnumerator InitUI()
    {
        Dictionary<string, GameUISO> uISO = new Dictionary<string, GameUISO>();
        windowPanelNames = new List<string>();
        //该场景下所有UI资产
        var handle = Addressables.LoadAssetAsync<GameUISO>(assetsLabel);
        handle.Completed += (gameUISo) =>
        {
            Debug.Log("GameUISO名称 " + gameUISo.Result.name);
            if (gameUISo.Result.uiType == UIType.Window)
            {
                windowPanelNames.Add(gameUISo.Result.name);
            }

            uISO.Add(gameUISo.Result.name, gameUISo.Result);
        };
        while (!handle.IsDone) yield return null;
        UIManager.Instance.InitDics(uISO);
        InitOpenUI();
        Addressables.Release(handle);
    }

    /// <summary>
    /// 初始化生成基础窗口UI
    /// </summary>
    private void InitOpenUI()
    {
        //查找所有基础UI，并打开
        var baseGameUISos = UIManager.Instance.gameUISODic.Values.ToList().FindAll(t => t.uiType == UIType.BasePanel);
        foreach (var baseGameUISo in baseGameUISos)
        {
            UIManager.Instance.OpenPanel(baseGameUISo.name);
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
            var windowGameUISos =
                UIManager.Instance.gameUISODic.Where(t => t.Value.uiType == UIType.Window).ToDictionary(p => p.Key);
            foreach (var openPanel in UIManager.Instance.openPanelDic)
            {
                if (windowGameUISos.ContainsKey(openPanel.Key))
                {
                    if (openPanel.Value.isShow)
                    {
                        UIManager.Instance.ClosePanel(openPanel.Key);
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(_sceneMenuName))
            {
                //没有菜单界面，也没有打开的窗口，启动退出界面
                var confirmPanel = UIManager.Instance.OpenPanel("ConfirmUI");
                if (confirmPanel is null) return;
                var confirmCanvas = confirmPanel as ConfirmWindowGUI;
                confirmCanvas.LoadConfirmWindowGUI("是否关闭软件！", UIManager.Instance.Exit, null);
                return;
            }

            if (UIManager.Instance.openPanelDic.TryGetValue(_sceneMenuName, out BasePanel menuPanel))
            {
                if (menuPanel.isShow)
                    UIManager.Instance.ClosePanel(_sceneMenuName);
                else
                    UIManager.Instance.OpenPanel(_sceneMenuName);
            }
        }
    }


    //测试

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

    /// <summary>
    /// 按键控制的界面
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="planeName"></param>
    private void KeyDownOpenPanel(KeyCode keyCode, string planeName)
    {
        if (Input.GetKeyDown(keyCode))
        {
            if (UIManager.Instance.openPanelDic.TryGetValue(planeName, out BasePanel taskPanel))
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
}