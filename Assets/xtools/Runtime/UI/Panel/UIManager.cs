using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace XTools.UI
{
    public class UIManager
    {
        //单例模式
        private static UIManager instance;

        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UIManager();
                }

                return instance;
            }
        }

        //ui地址字典
        public Dictionary<string, GameUISO> gameUISODic;
        public Dictionary<string, AssetReference> prefabDic;
        public Dictionary<string, BasePanel> openPanelDic;
        private Dictionary<string, AsyncOperationHandle<GameObject>> _handleDiC;

        //场景菜单窗口名称
        public string sceneMenuName;

        //UI预制体挂载节点
        private Transform uiRoot;

        public Transform UIRoot
        {
            get
            {
                if (uiRoot is null)
                {
                    uiRoot = new GameObject("AllCanvas").transform;
                }

                return uiRoot;
            }
        }

        //构造函数
        private UIManager()
        {
            prefabDic = new Dictionary<string, AssetReference>();
            openPanelDic = new Dictionary<string, BasePanel>();
            _handleDiC = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        }

        /// <summary>
        /// 传递UI预制体名称和地址进行初始化
        /// 使用CanvasManager等类进行UI数据整理，传入跟新地址
        /// </summary>
        /// <param name="uiPrefabPathDic"></param>
        public void InitDics(Dictionary<string, GameUISO> uiPrefabPathDic)
        {
            gameUISODic = new Dictionary<string, GameUISO>();
            foreach (var uiPath in uiPrefabPathDic)
            {
                gameUISODic.Add(uiPath.Key,uiPath.Value);
            }
        }


        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public BasePanel OpenPanel(string panelName)
        {
            BasePanel basePanel = null;
            //检查是否已经打开界面
            if (openPanelDic.TryGetValue(panelName, out basePanel))
            {
                if (!basePanel.isShow)
                {
                    if (basePanel.panelType == PanelType.PellucidityPanel)
                    {
                        var basePellucidityPanel = basePanel as BasePellucidityPanel;
                        basePellucidityPanel?.OpenPanel(panelName);
                    }
                    else if (basePanel.panelType == PanelType.ScalePanel)
                    {
                        var baseScalePanel = basePanel as BaseScalePanel;
                        baseScalePanel?.OpenPanel(panelName);
                    }
                    else
                    {
                        basePanel.OpenPanel(panelName);
                    }
                }
                else
                    CustomLogger.LogError("界面已经打开：" + panelName);

                return basePanel;
            }

           

            //检查是否存在预制体
            if (!prefabDic.ContainsKey(panelName))
            {
                //检查是否存在对应路径
                if (!gameUISODic.TryGetValue(panelName, out GameUISO panelSo))
                {
                    CustomLogger.LogError("界面名称错误或者未配置路径：" + panelName);
                    return null;
                }
                prefabDic.Add(panelName, panelSo.uiReference);
            }
            var assetReference = prefabDic[panelName];
            var handle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            handle.WaitForCompletion();
            GameObject currentPanelObj = GameObject.Instantiate(handle.Result,UIRoot);
            basePanel = currentPanelObj.GetComponent<BasePanel>();
            openPanelDic.Add(panelName, basePanel);
            basePanel.OpenPanel(panelName);
            _handleDiC.Add(panelName,handle);


            return basePanel;
        }

        //关闭窗口
        public bool ClosePanel(string panelName)
        {
            BasePanel currentPanel = null;
            if (!openPanelDic.TryGetValue(panelName, out currentPanel))
            {
                CustomLogger.LogError("界面未打开，不用关闭：" + panelName);
                return false;
            }

            if (currentPanel.isShow)
            {
                if (currentPanel.panelType == PanelType.PellucidityPanel)
                {
                    var basePellucidityPanel = currentPanel as BasePellucidityPanel;
                    basePellucidityPanel?.ClosePanel();
                }
                else if (currentPanel.panelType == PanelType.ScalePanel)
                {
                    var baseScalePanel = currentPanel as BaseScalePanel;
                    baseScalePanel?.ClosePanel();
                }
                else
                {
                    currentPanel.ClosePanel();
                }
            }
            else
                CustomLogger.LogError("界面已经关闭：" + panelName);

            return true;
        }

        //移除打开
        public void RemoveOpenPanel(string panelName)
        {
            if (openPanelDic.TryGetValue(panelName, out var panel))
            {
                openPanelDic.Remove(panelName);
            }
            //资源释放
            if (_handleDiC.TryGetValue(panelName, out var handle))
            {
                _handleDiC.Remove(panelName);
                Addressables.Release(handle);
            }
        }

        //退出
        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}