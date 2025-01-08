using System.Collections;
using UnityEngine;

namespace XTools.UI
{
    /// <summary>
    /// 协程控制窗口弹窗动画
    /// 适合使用频率较高的窗口
    /// 不删除物体
    /// </summary>
    public class BaseScalePanel : BasePanel
    {
        [Header("弹窗参数")] 
        //显示变化曲率
        [SerializeField] AnimationCurve showCurve;
        //隐藏变化曲率
        [SerializeField] AnimationCurve hideCurve;
        //变化时长
        [SerializeField] float animationSpeed = 1f;

        private Transform _childTransform;

        public Transform ChildTransform
        {
            get
            {
                if (_childTransform == null)
                {
                    _childTransform = transform.GetChild(0);
                    panelType = PanelType.ScalePanel;
                }

                return _childTransform;
            }
        }

        private bool _lock;
        
        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="nameStr"></param>
        public override void OpenPanel(string nameStr)
        {
            //base.OpenPanel(nameStr);
            if(_lock) return;
            StartCoroutine(ShowPlane(nameStr));
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public override void ClosePanel()
        {
            //base.ClosePanel();
            if(_lock) return;
            StartCoroutine(HidePlane());
        }
        
        /// <summary>
        /// 显示窗口，使用StartCoroutine启动
        /// </summary>
        /// <param name="nameStr"></param>
        /// <returns></returns>
        private IEnumerator ShowPlane(string nameStr)
        {
            _lock = true;
            this.name = nameStr;
            float time = 0;
            while (time <= 1f)
            {
                ChildTransform.localScale = Vector3.one * showCurve.Evaluate(time);
                time += Time.deltaTime * animationSpeed;
                yield return null;
            }

            ChildTransform.localScale = Vector3.one;
            isShow = true;
            openEvent?.Invoke();
            _lock = false;
        }

        /// <summary>
        /// 隐藏窗口，使用StartCoroutine启动
        /// </summary>
        /// <returns></returns>
        private  IEnumerator HidePlane()
        {
            _lock = true;
            float time = 0;
            while (time <= 1f)
            {
                ChildTransform.localScale = Vector3.one * hideCurve.Evaluate(time);
                time += Time.deltaTime * animationSpeed;
                yield return null;
            }
            ChildTransform.localScale = Vector3.zero;
            isShow = false;
            closeEvent?.Invoke();
            _lock = false;
        }
    }
}