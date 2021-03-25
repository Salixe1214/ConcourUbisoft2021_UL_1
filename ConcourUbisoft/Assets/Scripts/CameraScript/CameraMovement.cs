using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Voice;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float mouseSensivityX =120;
    [SerializeField] private float mouseSensivityY =120;
    [SerializeField] private float controllerSensivityX=120;
    [SerializeField] private float controllerSensivityY=120;
    [SerializeField] private float cameraRotationSmoothingSpeed = 0.7f;
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float mouseYAccumulator = 0f;
    private float mouseXAccumulator = 0f;
    private float controllerYAccumulator = 0f;
    private float controllerXAccumulator = 0f;
    private float xRotationControllerPS = 0f;
    private float yRotationControllerPS = 0f;
    private float xRotationControllerXBO = 0f;
    private float yRotationControllerXBO = 0f;
    private float accumulatedDeltaTime = 0.0f;
    
    private string[] joysticks;
    
    
    void Start()
    {
        joysticks = Input.GetJoystickNames();
        Cursor.lockState = CursorLockMode.Locked;
        mouseXAccumulator = 180;
        controllerXAccumulator = 180;
        xRotationControllerPS = 180;
        xRotationControllerXBO = 180;
    }

    //PS = playstation
    //XBO = xbox one
    void Update()
    {
        joysticks = Input.GetJoystickNames();
        RotateCamera();
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") *mouseSensivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensivityY;
        
        float controllerX_PS = Input.GetAxis("RightJoystickHorizontalPS")*controllerSensivityX*Time.deltaTime;
        float controllerY_PS = Input.GetAxis("RightJoystickVerticalPS")*controllerSensivityY*Time.deltaTime;
        float controllerX_XBO = Input.GetAxis("RightJoystickHorizontalXBO")*controllerSensivityX*Time.deltaTime;
        float controllerY_XBO = Input.GetAxis("RightJoystickVerticalXBO") * controllerSensivityY * Time.deltaTime;
        
        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
            controllerYAccumulator -= controllerY_XBO;
            controllerXAccumulator += controllerX_XBO;
            controllerYAccumulator = Mathf.Clamp(controllerYAccumulator, -90f, 90f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(controllerYAccumulator,0,0),cameraRotationSmoothingSpeed);
            playerBody.MoveRotation(Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z),cameraRotationSmoothingSpeed));
            //playerBody.MoveRotation(Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z));
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            controllerYAccumulator -= controllerY_PS;
            controllerXAccumulator += controllerX_PS;
            controllerYAccumulator = Mathf.Clamp(controllerYAccumulator, -90f, 90f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(controllerYAccumulator,0,0),cameraRotationSmoothingSpeed);
            playerBody.MoveRotation(Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z),cameraRotationSmoothingSpeed));
           // playerBody.MoveRotation(Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z));
        }
        else
        {
            mouseXAccumulator += mouseX;
            float previousMouseY = mouseYAccumulator;
            mouseYAccumulator -= mouseY;
            mouseYAccumulator = Mathf.Clamp(mouseYAccumulator, -90f, 90f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(mouseYAccumulator,0,0),cameraRotationSmoothingSpeed);
            playerBody.MoveRotation(Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(playerBody.rotation.x,mouseXAccumulator,playerBody.rotation.z),cameraRotationSmoothingSpeed));
           
        }
    }
}
