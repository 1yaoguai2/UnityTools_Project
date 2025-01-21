using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// OverlapSphereNonAlloc 很傻逼的一个API，首先查找的对象只能放到数组，数组大小无法修改，
/// 一定范围内随机查找到的对象放到数组，不一定是自己最需要的，多余数组大小的对象会被舍弃
/// 数组很大的其情况一般查找不满
/// </summary>
public class PhysicsOverlap : MonoBehaviour
{
    //目标层级
    public LayerMask layerMask;

    //差点到的对象
    public Collider[] colliders;

    //球形半径范围
    public float sphereaRdius;

    //是否查找最小距离
    public bool isMinDis;

    private Vector3 targetPosition;

    void Start()
    {
        colliders = new Collider[3];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetOverlapSphere();
        }
    }

    /// <summary>
    /// 球形范围检测
    /// </summary>
    private void GetOverlapSphere()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, sphereaRdius, colliders, layerMask);
        CustomLogger.Log("HitCount:" + hitCount);
        //colliders = Physics.OverlapSphere(transform.position, sphereaRdius,layerMask);
        //CustomLogger.Log("colliders.Length:" + colliders.Length);
        if (hitCount > 0)
        {
            float currentDis = isMinDis ? sphereaRdius + 1 : 0;
            for (int i = 0; i < hitCount; i++)
            {
                CustomLogger.Log("ObjectName:" + colliders[i].name);
                var targetPos = colliders[i].transform.position;
                var sqrResoult = Vector3.Distance(targetPos, transform.position);
                if (isMinDis)
                {
                    if (sqrResoult < currentDis)
                    {
                        currentDis = sqrResoult;
                        targetPosition = targetPos;
                    }
                }
                else
                {
                    if (sqrResoult > currentDis)
                    {
                        currentDis = sqrResoult;
                        targetPosition = targetPos;
                    }
                }
            }

            transform.position = targetPosition;
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 200, 40), "按下Q进行范围检测"))
        {
            GetOverlapSphere();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sphereaRdius);
    }
}