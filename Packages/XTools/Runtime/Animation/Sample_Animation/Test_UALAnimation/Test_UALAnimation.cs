using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Animation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Cat.Animation.Test
{
    public class Test_UALAnimation : MonoBehaviour
    {
        public Transform A;
        public Transform B;

        public float acceleration = 1;
        public float deceleration = 1;
        public float maxSpeed = 1;
        public float ratio_n = 1;

        private void Start()
        {
            TestUALAnim();
        }

        double theoryTime;
        double theoryFrameCount;
        Stopwatch sw = new Stopwatch();
        [ContextMenu("测试UAL动画")]
        void TestUALAnim()
        {
            sw.Start();
            var animN = new UALAnimation(acceleration,
                                        deceleration,
                                        maxSpeed,
                                        A.position,
                                        B.position,
                                        1,
                                        ratio_n);

            theoryTime = animN.ElapsedSecond;
            Debug.Log($"理论耗时{theoryTime}s");

            A.PlayAnimation(animN);

            animN.OnPlayComplete += AnimN_OnPlayComplete;
        }

        private void AnimN_OnPlayComplete(CatAnimation obj)
        {
            sw.Stop();
            Debug.Log($"开始时间{obj.StartTemeStemp}s");
            Debug.Log($"理论耗时{obj.ElapsedSecond}s");
            Debug.Log($"溢出{obj.OverSecond}s");
            Debug.Log($"物理{sw.Elapsed.TotalSeconds}s");

            Debug.Log($"{CatAnimationController.CurrentFrameCount * Time.fixedDeltaTime}");

            //Debug.Log($"相对理论，偏差{(double.Parse(total.ToString()) - theoryTime) / theoryTime:0.00%}");
        }
    }
}