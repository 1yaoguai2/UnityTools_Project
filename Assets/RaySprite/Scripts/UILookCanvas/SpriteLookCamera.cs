using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLookCamera : MonoBehaviour
{
    private Transform _cameraTrans;
    private void Start()
    {
        _cameraTrans = Camera.main.transform;
    }

    void FixedUpdate()
    {
        transform.LookAt(_cameraTrans);
    }
}
