#region Information
/*----------------------------------------------------------------
 * User    : Arno (Copyright (C))
 * Time    ：2021/1/8 09:37:03
 * CLR     ：4.0.30319.42000
 * filename：LineAnimation
 * E-male  ：iot_zx@163.com
 
 * Desc    : 匀加速直线运动
 *
 * ----------------------------------------------------------------
 * Modification
 *  - 至于怎么写的，重新温习下高中匀加速的物理课就完了，看这段代码是不好使的
 *  - 2021.01.27日
 *    - 发现有东西算错了，设可以最大速度，这是时候的t = Vtemp / a 
 *    - 总结出这个算法的误差来源就是下面这个开方数：
 *    Ta_all = Mathf.Sqrt(
                    (s * Mathf.Pow(d, 2)) / (a * Mathf.Pow(d, 2) / 2 + d * Mathf.Pow(a, 2) / 2)
                    );
 *----------------------------------------------------------------*/
#endregion

using System;
using System.Diagnostics;
using Animation;
using UnityEngine;

namespace Cat.Animation
{
    /// <summary>
    /// 匀加速直线运动（已知加速度，减速度，终点位置）
    /// Uniform Acceleration Line
    /// </summary>
    public class UALAnimation : CatAnimation
    {
        /// <summary>
        /// 匀加速直线运动配置
        /// </summary>
        /// <param name="acceleration">加速度</param>
        /// <param name="deceleration">减速度</param>
        /// <param name="maxSpeed">最大速度</param>
        /// <param name="consultPosition">运动计算参照物（用哪个位置来测算运动距离及方向）</param>
        /// <param name="targetPosition">目标位置</param>
        public UALAnimation(float acceleration,
                            float deceleration,
                            float maxSpeed,
                            Vector3 consultPosition,
                            Vector3 targetPosition,
                            float proportion = 1,
                            float ratio = 1)
        {
            if (deceleration <= 0)
                throw new ArgumentException("不要输入<=0的减速度,也没啥，就是不喜欢");
            this._a = acceleration;
            this._d = deceleration;//减速度在这里被取反了，所以对于外部而言，应该传入正值
            this._Vm = maxSpeed;
            this._start = consultPosition;
            this._end = targetPosition;
            this._proportion = proportion;
            this._ratio = ratio;

            CheckingCalculation();
        }
        /// <summary>
        /// 匀加速直线运动配置
        /// </summary>
        /// <param name="acceleration">加速度</param>
        /// <param name="deceleration">减速度</param>
        /// <param name="maxSpeed">最大速度</param>
        /// <param name="consultPosition">运动计算参照物（用哪个位置来测算运动距离及方向）</param>
        /// <param name="targetPosition">目标位置</param>
        /// <param name="axial">运动轴向</param>
        public UALAnimation(float acceleration,
                            float deceleration,
                            float maxSpeed,
                            Vector3 consultPosition,
                            Vector3 targetPosition,
                            LineAxial axial,
                            float proportion = 1,
                            float ratio = 1)

        {
            if (deceleration <= 0)
                throw new ArgumentException("不要输入<=0的减速度,也没啥，就是不喜欢");
            this._a = acceleration;
            this._d = deceleration;//减速度在这里被取反了，所以对于外部而言，应该传入正值
            this._Vm = maxSpeed;
            this._start = consultPosition;
            this._end = targetPosition;
            this._proportion = proportion;
            this._ratio = ratio;

            switch (axial)
            {
                case LineAxial.X:
                    this._end.y = this._start.y;
                    this._end.z = this._start.z;
                    break;
                case LineAxial.Y:
                    this._end.x = this._start.x;
                    this._end.z = this._start.z;
                    break;
                case LineAxial.Z:
                    this._end.x = this._start.x;
                    this._end.y = this._start.y;
                    break;
            }
            CheckingCalculation();
        }

        /// <summary>
        /// 【这个签名是本地坐标系！】匀加速直线运动配置
        /// </summary>
        /// <param name="acceleration">加速度</param>
        /// <param name="deceleration">减速度</param>
        /// <param name="maxSpeed">最大速度</param>
        /// <param name="consultPosition">运动计算参照物（用哪个位置来测算运动距离及方向）</param>
        /// <param name="targetPosition">目标位置</param>
        /// <param name="localAxial">本地运动轴向</param>
        /// <param name="consultRotation">运动计算参考物朝向，使用该值后，运动计算轴向将变成本地空间</param>
        public UALAnimation(float acceleration,
                            float deceleration,
                            float maxSpeed,
                            Vector3 consultPosition,
                            Vector3 targetPosition,
                            LineAxial localAxial,
                            Quaternion consultRotation,
                            float proportion = 1,
                            float ratio = 1)

        {
            if (deceleration <= 0)
                throw new ArgumentException("不要输入<=0的减速度,也没啥，就是不喜欢");
            this._a = acceleration;
            this._d = deceleration;//减速度在这里被取反了，所以对于外部而言，应该传入正值
            this._Vm = maxSpeed;
            this._start = consultPosition;
            this._end = targetPosition;
            this._proportion = proportion;
            this._ratio = ratio;

            Vector3 localDir = Vector3.zero;
            switch (localAxial)
            {
                case LineAxial.X:
                    localDir = consultRotation * Vector3.right;
                    break;
                case LineAxial.Y:
                    localDir = consultRotation * Vector3.up;
                    break;
                case LineAxial.Z:
                    localDir = consultRotation * Vector3.forward;
                    break;
            }
            var cos = Vector3.Dot(localDir, (_end - _start));
            this._end = _start + (localDir * cos);
            //Debug.DrawLine(this.start, this.end, Color.red, 88);
            CheckingCalculation();
        }

        /// <summary>
        /// 匀加速直线运动配置
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="deceleration"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="consultPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="oxyzPlane"></param>
        public UALAnimation(float acceleration,
                            float deceleration,
                            float maxSpeed,
                            Vector3 consultPosition,
                            Vector3 targetPosition,
                            LinePlane oxyzPlane,
                            float proportion = 1,
                            float ratio = 1)
        {
            if (deceleration <= 0)
                throw new ArgumentException("不要输入<=0的减速度,也没啥，就是不喜欢");
            this._a = acceleration;
            this._d = deceleration;//减速度在这里被取反了，所以对于外部而言，应该传入正值
            this._Vm = maxSpeed;
            this._start = consultPosition;
            this._end = targetPosition;
            this._proportion = proportion;
            this._ratio = ratio;

            switch (oxyzPlane)
            {
                case LinePlane.OXZ:
                    this._end.y = this._start.y;
                    break;
                case LinePlane.OZY:
                    this._end.x = this._start.x;
                    break;
                case LinePlane.OXY:
                    this._end.z = this._start.z;
                    break;
            }
            CheckingCalculation();
        }

        /// <summary>
        /// 【这个签名是本地坐标系！】匀加速直线运动配置
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="deceleration"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="consultPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="localOxyzPlane"></param>
        public UALAnimation(float acceleration,
                            float deceleration,
                            float maxSpeed,
                            Vector3 consultPosition,
                            Vector3 targetPosition,
                            LinePlane localOxyzPlane,
                            Quaternion consultRotation,
                            float proportion = 1,
                            float ratio = 1)
        {
            if (deceleration <= 0)
                throw new ArgumentException("不要输入<=0的减速度,也没啥，就是不喜欢");
            this._a = acceleration;
            this._d = deceleration;//减速度在这里被取反了，所以对于外部而言，应该传入正值
            this._Vm = maxSpeed;
            this._start = consultPosition;
            this._end = targetPosition;
            this._proportion = proportion;
            this._ratio = ratio;

            Vector3 normal;
            switch (localOxyzPlane)
            {
                case LinePlane.OXZ:
                    normal = consultRotation * Vector3.up;
                    break;
                case LinePlane.OZY:
                    normal = consultRotation * Vector3.right;
                    break;
                case LinePlane.OXY:
                    normal = consultRotation * Vector3.forward;
                    break;
                default: throw new Exception("plane");
            }
            //以normal为法线的平面，用第一个参数过去求dot
            //也可以理解为一个向量在这个平面上的投影
            var destination = Vector3.ProjectOnPlane(targetPosition - consultPosition, normal);
            this._end = this._start + destination;
            CheckingCalculation();
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
        float currentT = 0;//当前时间(已消耗的时间)
        readonly float timeSlice = Time.fixedDeltaTime;

        float surplusT = 0;//上一个阶段剩下的时间
        internal bool PlayUpdate()
        {
            if (_enableMaxV)//能够达到最大速度
            {
                if (_flow == 0)//加速阶段
                {
                    float deltaT;
                    //已经消耗的时间 加上 这一帧需要消耗的时间  减去 总时间  = 这一帧多消耗的时间
                    //当前阶段，在当前帧内，没用完的的时间
                    float overT = currentT + timeSlice - _Ta_all;
                    if (overT >= 0)
                    {
                        deltaT = timeSlice - overT;
                        currentT = 0;
                        _flow = 1;
                        surplusT = overT;

                        AccelerationPhase(deltaT * _ratio);
                    }
                    else
                    {
                        deltaT = timeSlice;
                        currentT += deltaT;
                        AccelerationPhase(deltaT * _ratio);
                    }
                }
                if (_flow == 1)//匀速阶段
                {
                    float deltaT;
                    if (surplusT > _Tm_all)//上一阶段剩下的时间够这一段
                    {
                        _flow = 2;
                        surplusT = surplusT - _Tm_all;//该阶段剩下的时间
                        UniformPhase(_Tm_all * _ratio);
                    }
                    else
                    {
                        if (surplusT > 0)
                        {
                            deltaT = surplusT;
                            currentT += deltaT;
                            UniformPhase(deltaT * _ratio);
                            surplusT = 0;
                            return false; //这一帧的剩余时间倍用完了，下一帧再来
                        }
                        else
                        {
                            float overT = currentT + timeSlice - _Tm_all;
                            if (overT >= 0)
                            {
                                deltaT = timeSlice - overT;
                                currentT = 0;
                                _flow = 2;
                                surplusT = overT;

                                UniformPhase(deltaT * _ratio);
                            }
                            else
                            {
                                deltaT = timeSlice;
                                currentT += deltaT;
                                UniformPhase(deltaT * _ratio);
                            }
                        }
                    }
                }
                if (_flow == 2)//减速阶段
                {
                    float deltaT;
                    if (surplusT > _Td_all)//上一阶段剩下的时间够这一段
                    {
                        _flow = 3;
                        surplusT = surplusT - _Td_all;//该阶段剩下的时间
                        DeceleratingPhase(_Td_all * _ratio);
                    }
                    else
                    {
                        if (surplusT > 0)
                        {
                            deltaT = surplusT;
                            currentT += deltaT;
                            DeceleratingPhase(deltaT * _ratio);
                            surplusT = 0;
                            return false; //这一帧的剩余时间倍用完了，下一帧再来
                        }
                        else
                        {
                            float overT = currentT + timeSlice - _Td_all;
                            if (overT >= 0)
                            {
                                deltaT = timeSlice - overT;
                                currentT = 0;
                                _flow = 3;

                                surplusT = overT;

                                DeceleratingPhase(deltaT * _ratio);

                                return false;//由于这最后一帧已经花掉了
                            }
                            else
                            {
                                deltaT = timeSlice;
                                currentT += deltaT;
                                DeceleratingPhase(deltaT * _ratio);
                            }
                        }
                    }
                }
            }
            else
            {
                if (_flow == 0)//加速阶段
                {
                    float deltaT;
                    //已经消耗的时间 加上 这一帧需要消耗的时间  减去 总时间  = 这一帧多消耗的时间
                    //当前阶段，在当前帧内，没用完的的时间
                    float overT = currentT + timeSlice - _Ta_all;
                    if (overT >= 0)
                    {
                        deltaT = timeSlice - overT;
                        currentT = 0;
                        _flow = 1;
                        surplusT = overT;

                        AccelerationPhase(deltaT * _ratio);
                    }
                    else
                    {
                        deltaT = timeSlice;
                        currentT += deltaT;
                        AccelerationPhase(deltaT * _ratio);
                    }
                }
                if (_flow == 1)//减速阶段
                {
                    float deltaT;
                    if (surplusT > _Td_all)//上一阶段剩下的时间够这一段
                    {
                        _flow = 3;
                        surplusT = surplusT - _Td_all;//该阶段剩下的时间
                        DeceleratingPhase(_Td_all * _ratio);
                    }
                    else
                    {
                        if (surplusT > 0)
                        {
                            deltaT = surplusT;
                            currentT += deltaT;
                            DeceleratingPhase(deltaT * _ratio);
                            surplusT = 0;
                            return false;
                        }
                        else
                        {
                            float overT = currentT + timeSlice - _Td_all;
                            if (overT >= 0)
                            {
                                deltaT = timeSlice - overT;
                                currentT = 0;
                                _flow = 3;

                                surplusT = overT;

                                DeceleratingPhase(deltaT * _ratio);

                                return false;//由于这最后一帧已经花掉了，所以这最后一帧不能直接报，要等下一帧
                            }
                            else
                            {
                                deltaT = timeSlice;
                                currentT += deltaT;
                                DeceleratingPhase(deltaT * _ratio);
                            }
                        }
                    }
                }
            }

            if (_flow == 3)
            {
                //这里不能直接赋值，
                //原因：外部如果提供的参考点，不是运动体本身，这样简单赋值是错误的
                //TransformSource.position = _end;
                OverSecond = surplusT;
                return true;
            }
            return false;
        }

        void AccelerationPhase(float deltaT)//在deltaT内，匀加速
        {
            var deltaS = _Vc * deltaT + (_a * Mathf.Pow((float)deltaT, 2) / 2);
            TransformSource.Translate(_dic * (float)deltaS, Space.World);
            _Vc += _a * deltaT;
        }
        void DeceleratingPhase(float deltaT)//在deltaT内，匀减速
        {
            var deltaS = _Vc * deltaT - (_d * Mathf.Pow((float)deltaT, 2) / 2);
            TransformSource.Translate(_dic * deltaS, Space.World);
            _Vc -= _d * deltaT;
        }
        void UniformPhase(float deltaT)//在deltaT内，匀速
        {
            var deltaS = _Vm * deltaT;
            TransformSource.Translate(_dic * (float)deltaS, Space.World);
        }


        readonly float _ratio = 1f;//倍率
        readonly float _a;//加速度
        readonly float _d;//减速度
        readonly float _Vm;//最大速度
        readonly Vector3 _start;//起点（供计算）
        readonly Vector3 _end;//终点（供计算）
        float _s;//总长度
        Vector3 _dic;//方向
        float _Sacc;//假如可以最大速度，那么达到最大速度后，加速要跑多长
        float _Sdec;//假如可以最大速度，那么达到最大速度后，减速要跑多长
        float _Svm;//假如可以最大速度，那么达到最大速度要跑多长
        float _Vc = 0;//当前速度
        float _Vtmp = 0;//如果不能最大速度，那么加速减速转换瞬间的速度
        float _Ta_all;//加速用时
        float _Tm_all;//匀速耗时
        float _Td_all;//减速用时

        int _flow = 0;//过程
        float _proportion = 1;//分量。1为全部，0.5为做一半，以此类推
        bool _enableMaxV = false;//能否达到最大速度（达到最大速度马上减速（临界），视为不能达到）
        /// <summary>
        /// 计算辅助参数
        /// </summary>
        void CheckingCalculation()
        {
            _dic = (_end - _start).normalized;
            _s = Vector3.Distance(_start, _end) * _proportion;

            _Ta_all = _Vm / _a;//从0加速到最大速度_耗时
            _Td_all = _Vm / _d;//从最大速度减速到0_耗时
            _Sacc = _a * Mathf.Pow(_Ta_all, 2) / 2;//从0加速到最大速度_距离
            _Sdec = _d * Mathf.Pow(_Td_all, 2) / 2;//从最大速度减速到0_距离

            if (_s - (_Sacc + _Sdec) <= 0)//达不到最大速度了
            {
                _Ta_all = Mathf.Sqrt(
                    (2 * _d * _s) / (_a * _d + Mathf.Pow(_a, 2))
                    );
                _Tm_all = 0;
                _Td_all = _a * _Ta_all / _d;
                _enableMaxV = false;
            }
            else//能够达到最大速度
            {
                _Svm = _s - _Sacc - _Sdec;//匀速距离
                _Tm_all = _Svm / _Vm;//匀速阶段耗时
                _enableMaxV = true;
            }

            //计算理论耗时(s)
            ElapsedSecond = (_Ta_all + _Tm_all + _Td_all) / _ratio;


            //后续的运动计算过程，是受倍率影响的
            _Ta_all /= _ratio;
            _Tm_all /= _ratio;
            _Td_all /= _ratio;



            //double v1 = 0;
            //double v2 = 0;
            //double v3 = 0;
            //if (_Ta_all % timeSlice != 0)
            //    v1 = 1 - _Ta_all % timeSlice / timeSlice;
            //if (_Tm_all % timeSlice != 0)
            //    v2 = 1 - _Tm_all % timeSlice / timeSlice;
            //if (_Td_all % timeSlice != 0)
            //    v3 = 1 - _Td_all % timeSlice / timeSlice;
            //OverFlowFrameCount += v1;
            //OverFlowFrameCount += v2;
            //OverFlowFrameCount += v3;


            //if (ElapsedTime.TotalMilliseconds < 100)
            //    Debug.LogWarning("动画倍率过高，或速度过大（3帧以内完成），" +
            //        "请不要使用任何统计时长的方式统计该动画，而使用[anim.ElapsedTime]来获得理论时长");
            //Debug.Log("UALAnim分析------");
            //Debug.Log($"实际总长:{_s},计算得出总长:{_Sacc + _Svm + _Sdec}");
            //Debug.Log($"加速：距离{_Sacc},时长 {1000 * _Ta_all }ms");
            //Debug.Log($"匀速：距离{_Svm}，时长 {1000 * _Tm_all }ms");
            //Debug.Log($"减速：距离{_Sdec}，时长 {1000 * _Td_all }ms");
            //Debug.Log($"预期时长 {ElapsedTime.TotalMilliseconds}ms");
            //Debug.Log("----------");
        }
    }
}