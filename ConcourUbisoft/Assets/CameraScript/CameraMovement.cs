using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Vector3 cameraTargetOffset;
    [SerializeField] private float cameraSmoothSpeed;
    
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = cameraTarget.position + cameraTargetOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, cameraSmoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(cameraTarget);
    }
}
