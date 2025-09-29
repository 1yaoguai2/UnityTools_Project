using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EquipController.Base;
using Cat.Animation;
using UnityEngine;

public enum RobotWorkStatus
{
    Idle,
    Take,
    Put
}

public class RobotMono : MonoBehaviour
{
    private List<Operator> _operators = new List<Operator>();
    private List<Operator> _operatorsElements = new List<Operator>();

    [SerializeField]
    private RobotWorkStatus workStatus = RobotWorkStatus.Idle;

    [SerializeField] private float forkSpeed = 1f; //伸夹抱松开速度
    [SerializeField] private float moveSpeed = 1f; //移动速度

    [SerializeField] private LineAxial _moveLineAxial;

    [Header("目标站台")]
    public Transform takeStation;
    public Transform putStation;
    
    [Header("目标货物")]
    public Transform targetCargo;

    //机械臂结构
    private Transform xz_A;
    private Transform xz_B;
    private Transform xz_C;
    private Transform xz_D;
    private Transform xz_E;
    private Transform leftFork;
    private Transform rightFork;
    private Transform startPoint;

    private Quaternion xzADefaultLocRotate;
    private Quaternion xzBDefaultLocRotate;
    private Quaternion xzCDefaultLocRotate;
    private Quaternion xzDDefaultLocRotate;
    private Quaternion xzEDefaultLocRotate;
    private Transform leftForkDefaultPoint;
    private Transform rightForkDefaultPoint;
    private Transform leftForkTargetPoint;
    private Transform rightForkTargetPoint;
    private Vector3 defaultRobotPos;

    [SerializeField] private float forkDis = 0.2f; //叉子距离

    [Header("速度")]
    //速度
    [SerializeField] private float xzASpeed = 1f;
    [SerializeField] private float xzBSpeed = 1f;
    [SerializeField] private float xzCSpeed = 1f;
    [SerializeField] private float xzDSpeed = 1f;
    [SerializeField] private float xzESpeed = 1f;


    private void Awake()
    {
        xz_A = transform.GetChild(0).GetChild(0);
        xz_B = xz_A.GetChild(0);
        xz_C = xz_B.GetChild(0);
        xz_D = xz_C.GetChild(0);
        xz_E = xz_D.GetChild(0);
        leftFork = xz_E.GetChild(0).Find("YD_L");
        rightFork = xz_E.GetChild(0).Find("YD_R");
        startPoint = xz_E.GetChild(0).Find("StartPoint");
        leftForkDefaultPoint = xz_E.GetChild(0).Find("L_Default");
        rightForkDefaultPoint = xz_E.GetChild(0).Find("R_Default");
        leftForkTargetPoint = xz_E.GetChild(0).Find("L_Target");
        rightForkTargetPoint = xz_E.GetChild(0).Find("R_Target");
        
        xzADefaultLocRotate = xz_A.localRotation;
        xzBDefaultLocRotate = xz_B.localRotation;
        xzCDefaultLocRotate = xz_C.localRotation;
        xzDDefaultLocRotate = xz_D.localRotation;
        xzEDefaultLocRotate = xz_E.localRotation;

        defaultRobotPos = transform.position;
    }

    void Update()
    {
        AnimRunning();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 30), "播放取货动画"))
        {
            StartCoroutine(WaiteIdle(true));
        }
        if (GUI.Button(new Rect(100, 200, 100, 30), "播放放货动画"))
        {
            StartCoroutine(WaiteIdle(false));
        }
    }

    private void AnimRunning()
    {
        if (_operators.Count > 0) //命令段
        {
            foreach (var t in _operators)
            {
                if (t.IsAllow())
                    t.Do();
            }

            var allComplete = false;
            foreach (var t in _operators)
            {
                allComplete = t.Status == OperatorStatus.Complete;
                if (!allComplete) break;
            }

            if (allComplete)
            {
                //耗时记录

                //MotionAnalysis.Push(crane, TimeSpan.FromSeconds(TotalElapsed).Multiply((int)m_rate));
                //Logger.Log($"提交时间{TotalElapsed}");

                _operators.Clear();
            }
        }

        if (_operatorsElements.Count > 0) //操作内容
        {
            foreach (var t in _operatorsElements)
            {
                if (t.IsAllow())
                    t.Do();
            }

            var allComplete = false;
            foreach (var t in _operatorsElements)
            {
                allComplete = t.Status == OperatorStatus.Complete;
                if (!allComplete) break;
            }

            if (!allComplete) return;

            _operatorsElements.Clear();
        }
    }

    //等待空闲再执行任务
    private IEnumerator WaiteIdle(bool isTake)
    {
        while (true)
        {
            if (workStatus == RobotWorkStatus.Idle)
            {
                if (isTake)
                {
                    PlayTakeAnim();
                }
                else
                {
                    PlayPutAnim();
                }

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 重置场景
    /// </summary>
    private void RestScene(bool isTake)
    {
        transform.position = defaultRobotPos;
        if (isTake)
        {
            targetCargo.parent = null;
            targetCargo.position = takeStation.position + new Vector3(0,1,0);
        }
        else
        {
            if (targetCargo.parent != startPoint)
            {
                targetCargo.parent = startPoint;
                targetCargo.localPosition = Vector3.zero;
            }
        }
    }

    public void PlayTakeAnim()
    {
        if (workStatus == RobotWorkStatus.Take) return;
        if (workStatus != RobotWorkStatus.Idle)
        {
            StartCoroutine(WaiteIdle(true));
            return;
        }

        RestScene(true);
        workStatus = RobotWorkStatus.Take;
        var op1 = new Operator();
        var op2 = new Operator();
        var op3 = new Operator();

        var takePoint = takeStation.position;
        MoveAnim(op1, takePoint);

        op1.OnComplete += () => { TakeAnim(op2, true); };
        op2.AddPrior(op1);

        op2.OnComplete += () => { MoveAnim(op3, defaultRobotPos); };

        op3.AddPrior(op2);
        op3.OnComplete += () =>
        {
            workStatus = RobotWorkStatus.Idle;
            CustomLogger.Log($"机器人{name}播放拆盖动画成功");
        };


        _operators.Add(op1);
        _operators.Add(op2);
        _operators.Add(op3);
    }

    public void PlayPutAnim()
    {
        if (workStatus == RobotWorkStatus.Put) return;
        if (workStatus != RobotWorkStatus.Idle)
        {
            StartCoroutine(WaiteIdle(false));
            return;
        }

        RestScene(false);
        workStatus = RobotWorkStatus.Put;
        var op1 = new Operator();
        var op2 = new Operator();

        MoveAnim(op1, putStation.position);

        op1.OnComplete += () => { TakeAnim(op2, false); };

        op2.AddPrior(op1);
        op2.OnComplete += () =>
        {
            workStatus = RobotWorkStatus.Idle;
            CustomLogger.Log($"机器人{name}播放放货动画结束");
        };

        _operators.Add(op1);
        _operators.Add(op2);
    }


    private void MoveAnim(Operator op, Vector3 target)
    {
        var anim = new LineAnimation(transform.position, target, moveSpeed, _moveLineAxial);
        anim.OnPlayComplete += (_) => { op.DoComplete(); };
        transform.PlayAnimation(anim);
    }

    private void TakeAnim(Operator op, bool isTake)
    {
        var op1 = new Operator();
        var op2 = new Operator();
        var op3 = new Operator();
        var op4 = new Operator();
        var op5 = new Operator();

        var takePoint = isTake ? targetRobotPoint["TakePoint"] : targetRobotPoint["PutPoint"];
        //int endIndex = 0;

        XZ_ARotate(op1, Quaternion.Euler(takePoint[0]));
        op1.OnComplete += () => { XZ_BCERotate(op2, Quaternion.Euler(takePoint[1]), Quaternion.Euler(takePoint[2])); };

        op2.AddPrior(op1);
        op2.OnComplete += () => { ForkAnim(op3, !isTake); };

        op3.AddPrior(op2);
        op3.OnComplete += () =>
        {
            XZ_BCERotate(op4, Quaternion.identity, Quaternion.identity);
        };

        op4.AddPrior(op3);
        op4.OnComplete += () => { XZ_ARotate(op5, Quaternion.identity); };

        op5.AddPrior(op4);
        op5.OnComplete += () =>
        {
            //XZ_BCERotate(op6, Quaternion.Euler(Vector3.zero), Quaternion.Euler(Vector3.zero));
            CustomLogger.Log($"机器人{name}播放取放货动画全部完成");
            op?.DoComplete();
        };

        _operatorsElements.Add(op1);
        _operatorsElements.Add(op2);
        _operatorsElements.Add(op3);
        _operatorsElements.Add(op4);
        _operatorsElements.Add(op5);
    }

    private void XZ_ARotate(Operator op, Quaternion rotationTarget)
    {
        IOCAnimation iocAnimation = new IOCAnimation();
        iocAnimation.CheckComplete += () =>
        {
            xz_A.localRotation = Quaternion.RotateTowards(xz_A.localRotation, rotationTarget, xzASpeed * Time.deltaTime);

            if (xz_A.localRotation == rotationTarget)
            {
                xz_A.localRotation = rotationTarget;
                return true;
            }

            return false;
        };
        iocAnimation.OnPlayComplete += e => { op?.DoComplete(); };
        xz_A.PlayAnimation(iocAnimation);
    }

    private void XZ_BRotate(Operator op, Quaternion rotationTarget)
    {
        IOCAnimation iocAnimation = new IOCAnimation();
        iocAnimation.CheckComplete += () =>
        {
            xz_B.localRotation = Quaternion.RotateTowards(xz_B.localRotation, rotationTarget, xzBSpeed * Time.deltaTime);

            if (xz_B.localRotation == rotationTarget)
            {
                xz_B.localRotation = rotationTarget;
                return true;
            }

            return false;
        };
        iocAnimation.OnPlayComplete += e => { op?.DoComplete(); };
        xz_B.PlayAnimation(iocAnimation);
    }

    private void XZ_CRotate(Operator op, Quaternion rotationTarget)
    {
        IOCAnimation iocAnimation = new IOCAnimation();
        iocAnimation.CheckComplete += () =>
        {
            xz_C.localRotation = Quaternion.RotateTowards(xz_C.localRotation, rotationTarget, xzCSpeed * Time.deltaTime);

            if (xz_C.localRotation == rotationTarget)
            {
                xz_C.localRotation = rotationTarget;
                return true;
            }

            return false;
        };
        iocAnimation.OnPlayComplete += e => { op?.DoComplete(); };
        xz_C.PlayAnimation(iocAnimation);
    }

    private void XZ_DRotate(Operator op, Quaternion rotationTarget)
    {
        IOCAnimation iocAnimation = new IOCAnimation();
        iocAnimation.CheckComplete += () =>
        {
            xz_D.localRotation = Quaternion.RotateTowards(xz_D.localRotation, rotationTarget, xzDSpeed * Time.deltaTime);

            if (xz_D.localRotation == rotationTarget)
            {
                xz_D.localRotation = rotationTarget;
                return true;
            }

            return false;
        };
        iocAnimation.OnPlayComplete += e => { op?.DoComplete(); };
        xz_D.PlayAnimation(iocAnimation);
    }

    private void XZ_ERotate(Operator op, Quaternion rotationTarget)
    {
        IOCAnimation iocAnimation = new IOCAnimation();
        iocAnimation.CheckComplete += () =>
        {
            xz_E.localRotation = Quaternion.RotateTowards(xz_E.localRotation, rotationTarget, xzESpeed * Time.deltaTime);

            if (xz_E.localRotation == rotationTarget)
            {
                xz_E.localRotation = rotationTarget;
                return true;
            }

            return false;
        };
        iocAnimation.OnPlayComplete += e => { op?.DoComplete(); };
        xz_E.PlayAnimation(iocAnimation);
    }

    private void ForkAnim(Operator op, bool isOpen)
    {
        var leftTarget =  isOpen ?  leftForkDefaultPoint.position: leftForkTargetPoint.position;
        var rightTarget = isOpen ? rightForkDefaultPoint.position : rightForkTargetPoint.position;

        var anim = new LineAnimation(leftFork.position, leftTarget, forkSpeed);
        anim.OnPlayComplete += (_) =>
        {
            TakeOrPutCargo(!isOpen);
            op?.DoComplete();
        };
        leftFork.PlayAnimation(anim);

        var anim1 = new LineAnimation(rightFork.position, rightTarget, forkSpeed);
        rightFork.PlayAnimation(anim1);
    }

    private void XZ_BCERotate(Operator op, Quaternion targetB, Quaternion targetC)
    {
        int endIndex = 0;

        var disRotation = Quaternion.identity;
        IOCAnimation iocAnimationE = new IOCAnimation();
        iocAnimationE.CheckComplete += () =>
        {
            disRotation = xz_B.localRotation * xz_C.localRotation;
            disRotation = Quaternion.Inverse(disRotation);
            xz_E.localRotation = disRotation;
            return false;
        };
        iocAnimationE.OnAbort += e => { op?.DoComplete(); };
        xz_E.PlayAnimation(iocAnimationE);

        IOCAnimation iocAnimationB = new IOCAnimation();
        iocAnimationB.CheckComplete += () =>
        {
            xz_B.localRotation = Quaternion.RotateTowards(xz_B.localRotation, targetB, xzBSpeed * Time.deltaTime);

            if (xz_B.localRotation == targetB)
            {
                xz_B.localRotation = targetB;
                return true;
            }

            return false;
        };
        iocAnimationB.OnPlayComplete += e =>
        {
            if (endIndex < 1) endIndex++;
            else
            {
                iocAnimationE.Abort();
            }
        };
        xz_B.PlayAnimation(iocAnimationB);

        IOCAnimation iocAnimationC = new IOCAnimation();
        iocAnimationC.CheckComplete += () =>
        {
            xz_C.localRotation = Quaternion.RotateTowards(xz_C.localRotation, targetC, xzCSpeed * Time.deltaTime);

            if (xz_C.localRotation == targetC)
            {
                xz_C.localRotation = targetC;
                return true;
            }

            return false;
        };
        iocAnimationC.OnPlayComplete += e =>
        {
            if (endIndex < 1) endIndex++;
            else
            {
                iocAnimationE.Abort();
            }
        };
        xz_C.PlayAnimation(iocAnimationC);
    }

    private void XZ_BCRotate(Operator op, Quaternion targetB, Quaternion targetC)
    {
        int endIndex = 0;

        IOCAnimation iocAnimationB = new IOCAnimation();
        iocAnimationB.CheckComplete += () =>
        {
            xz_B.localRotation = Quaternion.RotateTowards(xz_B.localRotation, targetB, xzBSpeed * Time.deltaTime);

            if (xz_B.localRotation == targetB)
            {
                xz_B.localRotation = targetB;
                return true;
            }

            return false;
        };
        iocAnimationB.OnPlayComplete += e =>
        {
            if (endIndex < 1) endIndex++;
            else
            {
                op?.DoComplete();
            }
        };
        xz_B.PlayAnimation(iocAnimationB);

        IOCAnimation iocAnimationC = new IOCAnimation();
        iocAnimationC.CheckComplete += () =>
        {
            xz_C.localRotation = Quaternion.RotateTowards(xz_C.localRotation, targetC, xzCSpeed * Time.deltaTime);

            if (xz_C.localRotation == targetC)
            {
                xz_C.localRotation = targetC;
                return true;
            }

            return false;
        };
        iocAnimationC.OnPlayComplete += e =>
        {
            if (endIndex < 1) endIndex++;
            else
            {
                op?.DoComplete();
            }
        };
        xz_C.PlayAnimation(iocAnimationC);
    }

    private void WaiteTime(Operator op, float time)
    {
        float timeDis = 0f;
        IOCAnimation timeAnim = new IOCAnimation();
        timeAnim.CheckComplete += () =>
        {
            if (timeDis > time)
            {
                return true;
            }

            timeDis += Time.deltaTime;
            return false;
        };
        timeAnim.OnPlayComplete += e => { op?.DoComplete(); };
        transform.PlayAnimation(timeAnim);
    }

    private void TakeOrPutCargo(bool isTake)
    {
        try
        {
            if (isTake)
            {
                targetCargo.SetParent(startPoint);
                CustomLogger.Log($"机器人{name}取货完成");
            }
            else
            {
                targetCargo.SetParent(null);
                CustomLogger.Log($"机器人{name}放货完成");
            }
        }
        catch (Exception e)
        {
            CustomLogger.LogError($"{name}机器人取放货失败：{e.Message}");
        }
    }


    private Dictionary<string, List<Vector3>> targetRobotPoint = new Dictionary<string, List<Vector3>>()
    {
        {
            "TakePoint", new List<Vector3>()
            {
                new Vector3(0, 90f, 0),
                new Vector3(0, 0, 50f),
                new Vector3(0, 0, 10f),
                new Vector3(0, 0, 0f),
                new Vector3(0, 0, 0f)
            }
        },
        {
            "PutPoint", new List<Vector3>()
            {
                new Vector3(0, -90f, 0),
                new Vector3(0, 0, 50f),
                new Vector3(0, 0, 10f),
                new Vector3(0, 0, 0f),
                new Vector3(0, 0, 0f)
            }
        }
    };
}