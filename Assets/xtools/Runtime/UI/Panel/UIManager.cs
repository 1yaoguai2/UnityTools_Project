using System.Collections.Generic;
using UnityEngine;

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
        public Dictionary<string, string> pathDic;
        public Dictionary<string, GameObject> prefabDic;
        public Dictionary<string, BasePanel> panelDic;

        //UI预制体挂载节点
        private Transform uiRoot;

        public Transform UIRoot
        {
            get
            {
                if (uiRoot == null)
                {
                    uiRoot = GameObject.Find("AllCanvas").transform;
                }

                return uiRoot;
            }
        }

        //构造函数
        private UIManager()
        {
            UIConst uiConst = new UIConst();
            InitDics(uiConst.uIPrefabPathsDic);
            prefabDic = new Dictionary<string, GameObject>();
            panelDic = new Dictionary<string, BasePanel>();
        }

        /// <summary>
        /// 传递UI预制体名称和地址进行初始化
        /// 使用CanvasManager等类进行UI数据整理，传入跟新地址
        /// </summary>
        /// <param name="uiPrefabPathDic"></param>
        public void InitDics(Dictionary<string, string> uiPrefabPathDic)
        {
            pathDic = new Dictionary<string, string>(uiPrefabPathDic);
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
            if (panelDic.TryGetValue(panelName, out basePanel))
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
                    Debug.LogError("界面已经打开：" + panelName);

                return basePanel;
            }

            //检查是否存在对应路径
            string path = string.Empty;
            if (!pathDic.TryGetValue(panelName, out path))
            {
                Debug.LogError("界面名称错误或者未配置路径：" + panelName);
                return null;
            }

            //检查是否存在预制体
            GameObject currentPanelObj = null;
            if (!prefabDic.TryGetValue(panelName, out currentPanelObj))
            {
                string realPath = "Prefabs/UI/" + path;
                currentPanelObj = Resources.Load<GameObject>(realPath);
                prefabDic.Add(panelName, currentPanelObj);
            }

            GameObject panelObj = GameObject.Instantiate(currentPanelObj, UIRoot, false);
            basePanel = panelObj.GetComponent<BasePanel>();
            panelDic.Add(panelName, basePanel);
            basePanel.isShow = true;
            return basePanel;
        }

        public bool ClosePanel(string panelName)
        {
            BasePanel currentPanel = null;
            if (!panelDic.TryGetValue(panelName, out currentPanel))
            {
                Debug.LogError("界面未打开，不用关闭：" + panelName);
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
                Debug.LogError("界面已经关闭：" + panelName);

            return true;
        }
    }


}