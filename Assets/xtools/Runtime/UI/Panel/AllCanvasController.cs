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
        PanelOpen();
    }

    /// <summary>
    /// 初始化UI预制体地址
    /// </summary>
    private IEnumerator InitUI()
    {
        Dictionary<string, GameUISO> uiSo = new Dictionary<string, GameUISO>();
        windowPanelNames = new List<string>();
        GameUISO gameUiSo;
        string uiName;
        //该场景下所有UI资产
        var handle = Addressables.LoadAssetAsync<GameUISO>(assetsLabel);
        handle.Completed += (gameUI) =>
        {
            gameUiSo = gameUI.Result;
            uiName = gameUiSo.name;
            CustomLogger.Log("GameUISo名称 " + uiName);
            if (gameUiSo.uiType == UIType.Window)
            {
                windowPanelNames.Add(uiName);
            }
            else if(gameUiSo.uiType == UIType.Menu)
            {
                _sceneMenuName = uiName;
            }

            uiSo.Add(uiName, gameUiSo);
        };
        while (!handle.IsDone) yield return null;
        UIManager.Instance.InitDics(uiSo);
        //查找所有基础UI，并打开
        var baseGameUISos = UIManager.Instance.gameUISODic.Values.ToList().FindAll(t => t.uiType == UIType.BasePanel);
        foreach (var baseGameUISo in baseGameUISos)
        {
            UIManager.Instance.OpenPanel(baseGameUISo.name);
        }
        //TODO:资源无法释放
        //Addressables.Release(handle);  
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
                if (confirmPanel is null)
                {
                    confirmPanel = UIManager.Instance.OpenPanel("ConfirmGUI");
                    if(confirmPanel is null) return;
                    var confirmGUIWindow = confirmPanel as ConfirmWindowGUI;
                    confirmGUIWindow.LoadConfirmWindowGUI("是否关闭软件！", UIManager.Instance.Exit, null);
                }
                else
                {
                    var confirmCanvas = confirmPanel as ConfirmWindow;
                    confirmCanvas.LoadConfirmWindow("是否关闭软件！", UIManager.Instance.Exit, null);
                }
               
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

}