using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float mouseSensivity;
    [SerializeField] private float controllerSensivity;
    
    private Vector3 velocity = Vector3.zero;
    private float xRotation = 0f;
    private float xRotationControllerPS = 0f;
    private float xRotationControllerXBO = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    //PS = playstation
    //XBO = xbox one
    void FixedUpdate()
    {

        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;
        float controllerX_PS = Input.GetAxis("RightJoystickHorizontalPS")*controllerSensivity*Time.deltaTime;
        float controllerY_PS = Input.GetAxis("RightJoystickVerticalPS")*controllerSensivity*Time.deltaTime;
        float controllerX_XBO = Input.GetAxis("RightJoystickHorizontalXBO")*controllerSensivity*Time.deltaTime;
        float controllerY_XBO = Input.GetAxis("RightJoystickVerticalXBO") * controllerSensivity * Time.deltaTime;

        string[] joysticks = Input.GetJoystickNames();

        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
               xRotationControllerXBO -= controllerY_XBO;
               xRotationControllerXBO = Mathf.Clamp(xRotationControllerXBO,-90f,90f);
               transform.localRotation = Quaternion.Euler(xRotationControllerXBO, 0f, 0f);
               playerBody.transform.Rotate(Vector3.up * controllerX_XBO);
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            xRotationControllerPS -= controllerY_PS; 
            xRotationControllerPS = Mathf.Clamp(xRotationControllerPS, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotationControllerPS, 0f, 0f);
            playerBody.transform.Rotate(Vector3.up * controllerX_PS);
        }
        else
        { 
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.transform.Rotate(Vector3.up*mouseX);
        }
        
    }
}
