using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cat.Animation.Test
{
    public class TestTimeSliceCounter : MonoBehaviour
    {
        [ContextMenu("测试")]
        public void Foo()
        {
            List<TimeSlice> timeSlices = new List<TimeSlice>()
            {
                new TimeSlice()
                {
                    left = 0,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.02M,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.04M,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.16M,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.16M,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.16M,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.32M,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.34M,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.40M,
                    right = 1M,
                },
                new TimeSlice()
                {
                    left = 0.54M,
                    right = 1M,
                },


            };

            var cost =  TimeSliceCounter.GetCost(timeSlices);
            Debug.Log(cost);
        }
    }
}
