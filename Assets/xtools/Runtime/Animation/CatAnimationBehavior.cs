using System;
using Cat.Animation;
using UnityEngine;

namespace Animation
{
    public class CatAnimationBehavior : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// CatAnimation的Mono执行器
        /// </summary>
        void FixedUpdate()
        {
            //这里不能用foreach，迭代器的值是不允许修改的，
            //这里执行过程中，用事件回调的方式修改了AnimationPlayingHandles的值，
            //这种写法骗过了编译器，然而并没有什么鸟用,运行过程中依然会提示Collection was modified

            var currentT = CatAnimationController.CurrentFrameCount * Time.fixedDeltaTime;

            var count = CatAnimationController.AnimationPlayingHandles.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                CatAnimation anim;
                try
                {
                    anim = CatAnimationController.AnimationPlayingHandles[i];
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.LogError("出错");
                    continue;
                }
                if (anim.TransformSource == null)//切换场景时，或者销毁时
                {
                    CatAnimationController.DeQueue(anim);//从动画集中移出
                    continue;
                }
                if (anim.State == CatAnimationStatus.Paused || anim.State == CatAnimationStatus.Abort)
                {
                    continue;
                }

                if (anim.State == CatAnimationStatus.None)
                {
                    //获得当前的时间（我的计数器时间）,表示动画开始
                    anim.StartTemeStemp = currentT;
                }

                anim.onPlayUpdate();

                if (anim.State == CatAnimationStatus.Completed)
                {
                    var start = decimal.Parse(anim.StartTemeStemp.ToString());
                    var end = decimal.Parse(currentT.ToString());
                    CatAnimationController.AnimCounter.Add(
                        new TimeSlice()
                        {
                            left = start,
                            right = end,
                            theoryTime = anim.ElapsedSecond
                        });
                    CatAnimationController.OverSecond += anim.OverSecond;

                    //完成以后出队
                    CatAnimationController.DeQueue(anim);
                }
            }
            CatAnimationController.CurrentFrameCount++;
        }
    }
}