using System;
using UnityEngine;

namespace Cat.Animation
{
    public enum CatAnimationStatus
    {
        None,
        Start,//第一帧
        Playing,
        Paused,
        Completed,
        Abort
    }
    public class CatAnimation
    {
        /// <summary>
        /// 需要运动的实体
        /// </summary>
        public Transform TransformSource;

        /// <summary>
        /// 耗时（数学公式计算得出的理论时长,受到倍率影响）
        /// 单位是秒，由于TimeSpan的精度，不能到微秒
        /// </summary>
        public float ElapsedSecond { get; protected set; }
        /// <summary>
        /// 动画开始时间戳
        /// </summary>
        public float StartTemeStemp { get; set; }
        /// <summary>
        /// 溢出时长（受倍率影响）
        /// </summary>
        public float OverSecond { get; protected set; }
        /// <summary>
        /// 耗时（理论时长,不受倍率印象）
        /// </summary>
        [Obsolete]
        public TimeSpan ElapsedTime { get; protected set; }
        /// <summary>
        /// 耗时（物理耗时）
        /// </summary>
        public TimeSpan RealElapsedTime { get; protected set; }
        /// <summary>
        /// 动画状态
        /// </summary>
        public CatAnimationStatus State { get; private set; } = CatAnimationStatus.None;

        /// <summary>
        /// 描述（调试用的标识）
        /// </summary>
        public string desc = "";

        public event Action<CatAnimation> OnPlayStart;
        public event Action<CatAnimation> OnPlayUpdate;
        public event Action<CatAnimation> OnPlayComplete;
        public event Action<CatAnimation> OnPause;
        public event Action<CatAnimation> OnContinue;
        public event Action<CatAnimation> OnAbort;
        internal void onPlayStart()
        {
            try
            {
                State = CatAnimationStatus.Start;
                OnPlayStart?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            
        }
        internal virtual void onPlayUpdate()
        {
            try
            {
                State = CatAnimationStatus.Playing;
                OnPlayUpdate?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            
        }
        internal void onPlayComplete()
        {
            try
            {
                State = CatAnimationStatus.Completed;
                OnPlayComplete?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        public void Pause()
        {
            try
            {
                //StateCache = State;
                State = CatAnimationStatus.Paused;
                OnPause?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        public void Continue()
        {
            try
            {
                State = CatAnimationStatus.Playing;
                OnContinue?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        public void Abort()
        {
            try
            {
                State = CatAnimationStatus.Abort;
                OnAbort?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            
        }
    }

}