using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayFindSprite : MonoBehaviour
{
    private Ray _ray;
    private RaycastHit _raycastHit;
    private Camera _camera;

    public Transform panelTrans;
    private Transform _target;
    private Vector3 _screenPos;
    [SerializeField] 
    private Vector3 _offsetScreenPos;
    
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CustomLogger.Log("鼠标左键按下");
            _ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _raycastHit, 10000))
            {
                if (_raycastHit.transform.CompareTag("Sprite"))
                {
                    _target = _raycastHit.transform;
                    CustomLogger.Log(_raycastHit.transform.position.ToString());
                }
            }
        }

        if (_target is not null)
        {
            _screenPos = _camera.WorldToScreenPoint(_target.position);
            panelTrans.position = _screenPos + _offsetScreenPos;
        }
    }
}
