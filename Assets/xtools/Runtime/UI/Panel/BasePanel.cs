using System;
using UnityEngine;

namespace XTools.UI
{
    /// <summary>
    /// 面板基础类
    /// 适合使用频率低的窗口
    /// 每次删除创建
    /// </summary>
    public class BasePanel : MonoBehaviour
    {
        //是否显示
        public bool isShow = false;

        //名称
        protected new string name;

        public PanelType panelType = PanelType.BasePanel;

        /// <summary>
        /// ！在窗口创建成功时绑定对应方法
        /// ！在窗口删除时移除所有方法
        /// </summary>
        public Action openEvent;

        /// <summary>
        /// ！在窗口创建成功时绑定对应方法
        /// ！在窗口删除时移除所有方法
        /// </summary>
        public Action closeEvent;

        //打开窗口
        public virtual void OpenPanel(string nameStr)
        {
            this.name = nameStr;
            gameObject.SetActive(true);
            isShow = true;
            openEvent?.Invoke();
        }

        //关闭窗口
        public virtual void ClosePanel()
        {
            isShow = false;
            gameObject.SetActive(false);
            closeEvent?.Invoke();
            openEvent = null;
            closeEvent = null;
            Destroy(gameObject);

            if (UIManager.Instance.panelDic.ContainsKey(name))
            {
                UIManager.Instance.panelDic.Remove(name);
            }
        }
    }

    public enum PanelType
    {
        BasePanel,
        ScalePanel,
        PellucidityPanel
    }
}