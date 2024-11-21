#region Information
/*----------------------------------------------------------------
 * User    : Arno (Copyright (C))
 * Time    ：2021/6/25
 * CLR     ：4.0.30319.42000
 * filename：LineAnimation
 * E-male  ：iot_zx@163.com
 
 * Desc    : 外部指定每一帧的朝向的运动
 *
 * ----------------------------------------------------------------
 * Modification
 *
 *----------------------------------------------------------------*/
#endregion

using System;
using System.Diagnostics;
using Animation;
using UnityEngine;

namespace Cat.Animation
{
    public delegate bool boolHandle();
    /// <summary>
    /// inversion of control动画，所有操作均又外部处理
    /// </summary>
    public class IOCAnimation : CatAnimation
    {
        public event boolHandle CheckComplete;
        public IOCAnimation()
        {
            
        }

        bool init = false;
        readonly Stopwatch sw = new Stopwatch();
        internal override void onPlayUpdate()
        {
            if (!init)
            {
                init = true;

                sw.Start();

                base.onPlayStart();
            }
            base.onPlayUpdate();//在每一帧操作之前，先响应外部的监听事件

            if (PlayUpdate())
            {
                sw.Stop();

                base.RealElapsedTime = sw.Elapsed;

                base.onPlayComplete();//运动完成后，上报
            }
        }

        private bool PlayUpdate()
        {
            return (bool)CheckComplete?.Invoke();
        }
    }
}
