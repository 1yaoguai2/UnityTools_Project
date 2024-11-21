using System;
using System.Collections.Generic;
using Animation;
using UnityEngine;

namespace Cat.Animation
{
    public static class CatAnimationController
    {
        /// <summary>
        /// 动画数量
        /// </summary>
        public static int AnimCount { get; set; }

        /// <summary>
        /// 当前动画帧总数
        /// </summary>
        public static int CurrentFrameCount { get; internal set; }

        /// <summary>
        /// 动画浪费掉的时间
        /// </summary>
        public static float OverSecond { get; set; }
        /// <summary>
        /// 计数器
        /// 左边是 是动画开始时间
        /// 右边是 是动画理论耗时
        /// </summary>
        public static List<TimeSlice> AnimCounter { get; private set; } = new List<TimeSlice>();

        /// <summary>
        /// 重置CatAnim的计数器
        /// </summary>
        public static void ResetCatAnimCounter()
        {
            AnimCounter.Clear();
            CurrentFrameCount = 0;
            AnimCount = 0;
            OverSecond = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>左边是总共消耗掉的时间，右边是中间空闲的时间</returns>
        public static (decimal, decimal) GetTotalTimeCost()
        {
            return TimeSliceCounter.GetCost(AnimCounter);
        }

        public static void PlayAnimation(this Transform source, CatAnimation anim)
        {
            //运动的本质是Transform(所以这个动画状态机，设计成了一个单transform的，也不知道以后会不会出现瓶颈)
            anim.TransformSource = source;

            //动画入队
            EnQueue(anim);

            //动画数量++
            AnimCount++;
        }

        public static List<CatAnimation> AnimationPlayingHandles { get; private set; } = new List<CatAnimation>();
        /// <summary>
        /// 入队（将动画放置执行队列），这个队列，没有先后概念
        /// </summary>
        static void EnQueue(CatAnimation animation)
        {
            //加入到update队列
            AnimationPlayingHandles.Add(animation);
        }
        internal static void DeQueue(CatAnimation animation)
        {
            //从update队列中移出
            AnimationPlayingHandles.Remove(animation);
        }
        static CatAnimationController()
        {
            //Debug.Log("编译动画控制器");
            //检查Unity当前Hierarchy面板上有没有一个CatAnimation
            CatAnimationBehavior[] cats = UnityEngine.Object.FindObjectsOfType<CatAnimationBehavior>();
            if (cats.Length > 1)
                throw new Exception("当前场景中存在多个" + nameof(CatAnimationBehavior) + ",合理操作是一个都不挂");
            if (cats.Length == 0)
            {
                GameObject g = new GameObject("@" + nameof(CatAnimationBehavior));
                g.AddComponent<CatAnimationBehavior>();
                if (GameObject.Find("@Cat"))
                    g.transform.SetParent(GameObject.Find("@Cat").transform);
            }
        }
    }
}