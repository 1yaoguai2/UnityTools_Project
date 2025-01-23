using System;
using UnityEngine;

public class RotationModel : MonoBehaviour
{
    private Transform modelTransform;

    private bool isRotate;

    public bool IsRotate
    {
        get => isRotate;
        set
        {
            if (isRotate != value)
            {
                isRotate = value;
                if (value)
                {
                    startPoint = Input.mousePosition;
                    startAngle = modelTransform.eulerAngles;
                }
            }
        }
    }

    private Vector3 startPoint;
    private Vector3 startAngle;

    [SerializeField] [Range(0.1f, 0.3f)] private float rotationSpeed;
    [SerializeField] [Range(0.01f, 0.1f)] private float scaleSpeed;

    [Header("Event")] public VoidEventSO scaleModelEventSo;
    public VectorEventSO rotationEventSo;

    void Start()
    {
        modelTransform = transform;
        scaleModelEventSo.OnRaiseEvent += ScaleModelEvent;
        rotationEventSo.OnRaiseEvent += RotationModelEvent;
    }

    private void OnDisable()
    {
        scaleModelEventSo.OnRaiseEvent -= ScaleModelEvent;
        rotationEventSo.OnRaiseEvent -= RotationModelEvent;
    }


    private void FixedUpdate()
    {
        if (!isRotate)
        {
            if (modelTransform.eulerAngles.y != 0)
            {
                modelTransform.rotation =
                    Quaternion.Lerp(modelTransform.rotation, Quaternion.identity, rotationSpeed / 10f);
            }

            if (modelTransform.localScale.x > 1)
            {
                modelTransform.localScale -= scaleSpeed / 10f * Vector3.one;
            }
        }
    }

    /// <summary>
    /// 旋转模型
    /// </summary>
    /// <param name="v"></param>
    private void RotationModelEvent((float, float) v)
    {
        var currentPoint = new Vector2(v.Item1, v.Item2);
        var x = (startPoint.x - currentPoint.x) * rotationSpeed;
        var y = (currentPoint.y - startPoint.y) * rotationSpeed;
        modelTransform.eulerAngles = startAngle + new Vector3(y, x, 0);
    }

    /// <summary>
    /// 缩放模型
    /// </summary>
    private void ScaleModelEvent()
    {
        var size = modelTransform.localScale;
        //size += (scrollWheel > 0 ? 1 : -1) * scaleSpeed  * Vector3.one;
        //只能放大
        size += scaleSpeed * Vector3.one;
        modelTransform.localScale = size;
    }

    //模型展示
    public void ShowRole_ClickEvent(int index)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == index)
                transform.GetChild(i).gameObject.SetActive(true);
            else
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}