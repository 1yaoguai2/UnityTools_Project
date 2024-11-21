using System.Diagnostics;
using UnityEngine;

namespace Cat.Animation
{
    /// <summary>
    /// 匀速直线运动
    /// </summary>
    public class LineAnimation : CatAnimation
    {
        Vector3 _start;
        Vector3 _end;
        float _speed;
        Vector3 _dir;
        float _s;
        float _surplus;
        float _ratio = 1f;//倍率
        void Construction(Vector3 consultPosition, Vector3 targetPosition, float speed, float proportion = 1, float ratio = 1)
        {
            speed = speed <= 0 ? throw new System.ArgumentException(nameof(LineAnimation) + "The speed needs to be a non-zero positive integer") : speed;
            _start = consultPosition;
            _end = targetPosition;
            _speed = speed;
            _ratio = ratio;
            CheckingCalculation(proportion);
        }
        /// <summary>
        /// 线性匀速运动
        /// </summary>
        /// <param name="consultPosition">起点</param>
        /// <param name="targetPosition">终点</param>
        /// <param name="speed">速度(m/s)</param>
        public LineAnimation(Vector3 consultPosition,
                             Vector3 targetPosition,
                             float speed,
                             float proportion = 1,
                             float ratio = 1)
        {
            Construction(consultPosition, targetPosition, speed, proportion, ratio);
        }
        /// <summary>
        /// 线性匀速运动
        /// </summary>
        /// <param name="consultPosition">起点</param>
        /// <param name="targetPosition">终点</param>
        /// <param name="speed">速度(m/s)</param>
        /// <param name="axial">轴向</param>
        /// <param name="proportion">分量（0.5，就是只运动一半的距离）</param>
        public LineAnimation(Vector3 consultPosition,
                             Vector3 targetPosition,
                             float speed,
                             LineAxial axial,
                             float proportion = 1,
                             float ratio = 1)
        {
            switch (axial)
            {
                case LineAxial.X:
                    targetPosition.y = consultPosition.y;
                    targetPosition.z = consultPosition.z;
                    //targetPosition.x = consultPosition.x + (targetPosition.x - consultPosition.x) * proportion;
                    break;
                case LineAxial.Y:
                    targetPosition.x = consultPosition.x;
                    targetPosition.z = consultPosition.z;
                    //targetPosition.y = consultPosition.y + (targetPosition.y - consultPosition.y) * proportion;
                    break;
                case LineAxial.Z:
                    targetPosition.x = consultPosition.x;
                    targetPosition.y = consultPosition.y;
                    //targetPosition.z = consultPosition.z + (targetPosition.z - consultPosition.z) * proportion;
                    break;
            }
            Construction(consultPosition, targetPosition, speed, proportion, ratio);
        }
        /// <summary>
        /// 【参考本地坐标系！】线性匀速运动
        /// </summary>
        /// <param name="consultPosition">起点</param>
        /// <param name="targetPosition">终点</param>
        /// <param name="speed">速度(m/s)</param>
        /// <param name="localAxial">轴向</param>
        /// <param name="consultRotation">参考物的本地旋转</param>
        /// <param name="proportion">分量（0.5，就是只运动一半的距离）</param>
        public LineAnimation(Vector3 consultPosition,
                             Vector3 targetPosition,
                             float speed,
                             LineAxial localAxial,
                             Quaternion consultRotation,
                             float proportion = 1,
                             float ratio = 1)
        {
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
            var t = Vector3.Dot(localDir, (targetPosition - consultPosition));
            targetPosition = consultPosition + (localDir * t);
            //Debug.DrawLine(this.consultPosition, this.targetPosition, Color.red, 88);
            Construction(consultPosition, targetPosition, speed, proportion, ratio);
        }
        /// <summary>
        /// 线性匀速运动
        /// </summary>
        /// <param name="consultPosition">起点</param>
        /// <param name="targetPosition">终点</param>
        /// <param name="speed">速度(m/s)</param>
        /// <param name="plane">面</param>
        public LineAnimation(Vector3 consultPosition,
                             Vector3 targetPosition,
                             float speed,
                             LinePlane plane,
                             float proportion = 1,
                             float ratio = 1)
        {
            switch (plane)
            {
                case LinePlane.OXZ:
                    targetPosition.y = consultPosition.y;
                    break;
                case LinePlane.OZY:
                    targetPosition.y = consultPosition.x;
                    break;
                case LinePlane.OXY:
                    targetPosition.y = consultPosition.z;
                    break;
            }
            Construction(consultPosition, targetPosition, speed, proportion, ratio);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consultPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="speed">(m/s)</param>
        /// <param name="localOxyzPlane"></param>
        /// <param name="consultRotation"></param>
        /// <param name="proportion"></param>
        public LineAnimation(Vector3 consultPosition,
                             Vector3 targetPosition,
                             float speed,
                             LinePlane localOxyzPlane,
                             Quaternion consultRotation,
                             float proportion = 1,
                             float ratio = 1)
        {
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
                default: throw new System.Exception("plane");
            }
            //以normal为法线的平面，用第一个参数过去求dot
            //也可以理解为一个向量在这个平面上的投影
            var destination = Vector3.ProjectOnPlane(targetPosition - consultPosition, normal);
            targetPosition = consultPosition + destination;
            Construction(consultPosition, targetPosition, speed, proportion, ratio);
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
            float nowdis = Time.fixedDeltaTime * _speed;
            nowdis = _surplus + nowdis > _s ? _s - _surplus : nowdis;//速度过大，那就一帧到位
            base.TransformSource.Translate(_dir * nowdis, Space.World);
            _surplus += nowdis;
            if (_surplus >= _s)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 计算辅助变量
        /// </summary>
        void CheckingCalculation(float proportion)
        {
            var v = (_end - _start);
            _dir = v.normalized;
            _s = v.magnitude * proportion;

            //计算耗时
            ElapsedSecond = _s / _speed;

            //后续运动考虑倍率
            _speed *= _ratio;

            //计算越界
            //var delta = Time.fixedDeltaTime;
            //OverFlowTime = 1 - _s / _speed % delta / delta;
        }
    }
}