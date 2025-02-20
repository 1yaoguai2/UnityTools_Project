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
        var handle = Addressables.LoadAssetsAsync<GameUISO>(assetsLabel, (obj) =>
        {
            CustomLogger.Log(obj.ToString());
        },true);
        handle.Completed += (gameUI) =>
        {
            for (int i = 0; i < gameUI.Result.Count; i++)
            {
                gameUiSo = gameUI.Result[i];
                uiName = gameUiSo.name;
                CustomLogger.Log("GameUISo名称 " + uiName);
                if (gameUiSo.uiType == UIType.Window)
                {
                    windowPanelNames.Add(uiName);
                }
                else if (gameUiSo.uiType == UIType.Menu)
                {
                    UIManager.Instance.sceneMenuName = _sceneMenuName = uiName;
                }

                uiSo.Add(uiName, gameUiSo);
            }
           
        };
        while (!handle.IsDone) yield return null;
        UIManager.Instance.InitDics(uiSo);
        //查找所有基础UI，并打开
        var baseGameUISos = UIManager.Instance.gameUISODic.Values.ToList().FindAll(t => t.uiType == UIType.BasePanel);
        foreach (var baseGameUISo in baseGameUISos)
        {
            UIManager.Instance.OpenPanel(baseGameUISo.name);
        }

        Addressables.Release(handle);
    }


    

#if ENABLE_INPUT_SYSTEM
    //关闭某些窗口
    public void CloseSomePanel(InputAction.CallbackContext obj)
    {
        EscapeDownOpenOrClosePanel();
    }
#else
    private void Update()
    {
        PanelOpen();
    }
    // 窗口跟新
    private void PanelOpen()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) EscapeDownOpenOrClosePanel();
    }
#endif


    /// <summary>
    /// Escape按键的一些功能
    /// </summary>
    private void EscapeDownOpenOrClosePanel()
    {
        var windowGameUISos =
            UIManager.Instance.gameUISODic.Where(t => t.Value.uiType == UIType.Window).ToDictionary(p => p.Key);
        foreach (var openPanel in UIManager.Instance.openPanelDic.Where(openPanel => 
                     windowGameUISos.ContainsKey(openPanel.Key)).Where(openPanel => openPanel.Value.isShow))
        {
            UIManager.Instance.ClosePanel(openPanel.Key);
            return;
        }

        if (string.IsNullOrEmpty(_sceneMenuName))
        {
            //没有菜单界面，也没有打开的窗口，启动退出界面
            var customUIBool = UIManager.Instance.gameUISODic.ContainsKey("ConfirmUI");
            if (customUIBool)
            {
                var confirmPanel = UIManager.Instance.OpenPanel("ConfirmUI");
                var confirmCanvas = confirmPanel as ConfirmWindow;
                confirmCanvas.LoadConfirmWindow("是否关闭软件！", UIManager.Instance.Exit, null);
            }
            else
            {
                var confirmPanel = UIManager.Instance.OpenPanel("ConfirmGUI");
                var confirmGUIWindow = confirmPanel as ConfirmWindowGUI;
                confirmGUIWindow.LoadConfirmWindowGUI("是否关闭软件！", UIManager.Instance.Exit, null);
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
        else
        {
            UIManager.Instance.OpenPanel(_sceneMenuName);
        }
    }
}