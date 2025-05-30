using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera_Label : MonoBehaviour
{
    public Transform currentCamera;
    void Start()
    {
        if (currentCamera == null)
            currentCamera = GameObject.FindAnyObjectByType<Camera>().transform;
    }

    private void FixedUpdate()
    {
        //transform.LookAt(_camera);
        LookCamear();
    }

    void LookCamear()
    {
        var dir = transform.position - currentCamera.transform.position;
        //dir.y = 0;
        var q = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, 1f);
    }

}
