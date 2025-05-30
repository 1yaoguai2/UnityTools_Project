using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 1f;

    private void FixedUpdate()
    {
        transform.RotateAround(new Vector3(0f, 0f, 0f), Vector3.up, Time.deltaTime * rotationSpeed);
    }
}