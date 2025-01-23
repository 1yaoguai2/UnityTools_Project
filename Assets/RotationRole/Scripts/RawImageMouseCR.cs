using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RawImageMouseCR : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler,IPointerEnterHandler,IPointerExitHandler
{
    public RotationModel rotationModel;
    [Header("Event")]
    public VectorEventSO rotationModelEventSo;
    public VoidEventSO scaleModelEventSo;


    public void OnPointerDown(PointerEventData eventData)
    {
        //CustomLogger.Log("鼠标进入并按下");
        rotationModel.IsRotate = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rotationModel.IsRotate = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(OnMouseOnTop());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            var currentPoint = Input.mousePosition;
            (float, float) f = (currentPoint.x, currentPoint.y);
            rotationModelEventSo.RaisedEvent(f);
        }
    }


    //鼠标在Ui上面
    public IEnumerator OnMouseOnTop()
    {
        CustomLogger.Log("鼠标在UI上面");
        while (true)
        {
            ScrollWheelControl();
            yield return null;    
        }
        
    }
    
    /// <summary>
    /// 鼠标滚轮控制视角远近
    /// </summary>
    public void ScrollWheelControl()
    {
        var scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {
            scaleModelEventSo.RaisedEvent();
        }
    }

  
}