using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cat.Animation
{
    /// <summary>
    /// 描述动画的开始时间，和经过数学理论耗时的结束时间
    /// </summary>
    public class TimeSlice
    {
        /// <summary>
        /// 实际开始时间
        /// </summary>
        public decimal left;
        /// <summary>
        /// 实际结束时间
        /// </summary>
        public decimal right;

        /// <summary>
        /// 理论耗时
        /// </summary>
        public float theoryTime;

        /// <summary>
        /// 相交
        /// </summary>
        /// <param name="slice"></param>
        /// <returns></returns>
        public bool Crossing(in TimeSlice slice)
        {
            return left <= slice.right && right >= slice.left;
        }
    }
    public static class TimeSliceCounter
    {
        public static (decimal, decimal) GetCost(in List<TimeSlice> timeSlices)
        {
            int count = timeSlices.Count;

            if (count == 0)
                return default;
            else if (count == 1)
            {
                var t = timeSlices[0];
                return (t.right - t.left, 0);
            }


            static int Comparison(TimeSlice a, TimeSlice b)
            {
                if (a.left < b.left)
                    return -1;
                else if (a.left > b.left)
                    return 1;
                return 0;
            };

            timeSlices.Sort(Comparison);

            decimal gapT = 0;


            for (int i = 0; i < count - 1; i++)
            {
                var a = timeSlices[i];
                var b = timeSlices[i + 1];
                if (a.Crossing(b))//相交
                {
                    continue;
                }
                else//不相交
                {
                    gapT += b.left - a.right;
                    break;
                }
            }


            decimal totalT = timeSlices[timeSlices.Count - 1].right - timeSlices[0].left;

            return (totalT, gapT);
        }
    }
}