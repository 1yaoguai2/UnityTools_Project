using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity扩展类
/// Transform
/// </summary>
public static class TransformExtension
{
    /// <summary>
    /// 获取Transform全部一级子物体
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="oneLevelChild"></param>
    public static void GetAllOneLevelChilds(this Transform transform, List<Transform> oneLevelChild)
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                oneLevelChild.Add(transform.GetChild(i));
            }
        }
    }

    /// <summary>
    /// 设置新层级
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layerId"></param>
    /// <param name="childLayerChange"></param>
    public static void SetLayerMask(this Transform transform, int layerId, bool childLayerChange = false)
    {
        transform.gameObject.layer = layerId;
        if (childLayerChange)
        {
            foreach (var trans in transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layerId;
            }
        }
    }

    /// <summary>
    /// 设置新层级
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layerStr"></param>
    /// <param name="childLayerChange"></param>
    public static void SetLayerMask(this Transform transform, string layerStr, bool childLayerChange = false)
    {
        int layerId = LayerMask.NameToLayer(layerStr);
        transform.gameObject.layer = layerId;
        if (childLayerChange)
        {
            foreach (var trans in transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layerId;
            }
        }
    }

    /// <summary>
    /// 设置世界坐标X
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="value"></param>
    public static void SetPositionX(this Transform transform, float value, bool isLocal = false)
    {
        Vector3 originalPos = isLocal ? transform.localPosition : transform.position;
        originalPos.x = value;
        if (isLocal) transform.localPosition = originalPos;
        else transform.position = originalPos;
    }

    /// <summary>
    /// 设置世界坐标Y
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="value"></param>
    public static void SetPositionY(this Transform transform, float value, bool isLocal = false)
    {
        Vector3 originalPos = isLocal ? transform.localPosition : transform.position;
        originalPos.y = value;
        if (isLocal) transform.localPosition = originalPos;
        else transform.position = originalPos;
    }

    /// <summary>
    /// 设置世界坐标Z
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="value"></param>
    public static void SetPositionZ(this Transform transform, float value, bool isLocal = false)
    {
        Vector3 originalPos = isLocal ? transform.localPosition : transform.position;
        originalPos.z = value;
        if (isLocal) transform.localPosition = originalPos;
        else transform.position = originalPos;
    }

    #region Transform Operations
    /// <summary>
    /// 重置变换
    /// Reset transform
    /// </summary>
    public static void ResetTransform(this Transform trans)
    {
        trans.transform.localPosition = Vector3.zero;
        trans.transform.localRotation = Quaternion.identity;
        trans.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 销毁所有子对象
    /// Destroy all children
    /// </summary>
    public static void DestroyChildren(this Transform trans)
    {
        var transform = trans.transform;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                GameObject.Destroy(transform.GetChild(i).gameObject);
            else
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    #endregion
}
