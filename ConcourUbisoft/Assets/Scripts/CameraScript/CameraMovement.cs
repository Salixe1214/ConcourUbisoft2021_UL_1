using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Vector3 cameraTargetOffset;
    [SerializeField] private float cameraSmoothSpeed;
    
    private Vector3 velocity = Vector3.zero;
   

    void LateUpdate()
    {
        Vector3 desiredPosition = cameraTarget.position + cameraTargetOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, cameraSmoothSpeed *Time.deltaTime);
        //Vector3 smoothedPosition =Vector3.Slerp(transform.position, desiredPosition, cameraSmoothSpeed);
        transform.position = smoothedPosition;
        var targetRotation = Quaternion.LookRotation(cameraTarget.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSmoothSpeed * Time.deltaTime);
        transform.LookAt(cameraTarget.transform);
    }
}
