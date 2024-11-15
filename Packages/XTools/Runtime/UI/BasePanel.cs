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
        

        //打开窗口
        public virtual void OpenPanel(string nameStr)
        {
            this.name = nameStr;
            gameObject.SetActive(true);
            isShow = true;
        }

        //关闭窗口
        public virtual void ClosePanel()
        {
            isShow = false;
            gameObject.SetActive(false);
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