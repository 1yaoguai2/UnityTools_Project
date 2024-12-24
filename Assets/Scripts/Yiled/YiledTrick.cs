using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yield
{

    public class YiledTrick : MonoBehaviour
    {
        public int TestCase = 0;

        /// <summary>
        /// 将start方法的返回值类型，修改为IEnumbertor，start会变成一个协程方法
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            CustomLogger.Log($"Start开始协程，当前帧{Time.frameCount}");
            switch (TestCase)
            {
                //start改为协程后，测试等待执行
                case 0:
                    yield return null;
                    Debug.Log($"Start开始协程等待后，当前帧:{Time.frameCount}");
                    break;
                    case 1: 
                    yield return new WaitForEndOfFrame(); //new新对象，产生GC
                    Debug.Log($"Yiled return EndOfFeame,1，当前帧:{Time.frameCount}");
                    yield return YieldHelper.WaitForEndOfFrame;  //静态对象重复利用
                    Debug.Log($"Yiled return EndOfFeame,2，当前帧:{Time.frameCount}");
                    yield return YieldHelper.WaitForEndOfFrame;
                    Debug.Log($"Yiled return EndOfFeame,3，当前帧:{Time.frameCount}");
                    break;
                    case 2:
                    yield return new WaitForSeconds(1f); //产生GC
                    Debug.Log($"Yiled return WaitForSeconds,1，当时间time:{Time.time}");
                    yield return YieldHelper.WaitForSeconds(1f); //静态方法，重复利用
                    Debug.Log($"Yiled return WaitForSeconds,2，当时间time:{Time.time}");
                    yield return YieldHelper.WaitForSeconds(1f); 
                    Debug.Log($"Yiled return WaitForSeconds,3，当时间time:{Time.time}");
                    break;
                case 3:
                    yield return 0;  //无论返回null，0，100，都等地一帧
                    Debug.Log($"Yiled return (int),1，当前帧:{Time.frameCount}");
                    yield return 100;
                    Debug.Log($"Yiled return (int),2，当前帧:{Time.frameCount}");
                    yield return YieldHelper.WaitForFrame(10);  //等待10帧
                    CustomLogger.Log($"Yiled return YieldHelper.WaitForFrame(10),3，当前帧:{Time.frameCount}");
                    break;
                default:
                    CustomLogger.Log("TestCase不存在！");
                    break;
            }
        }
    }
}
