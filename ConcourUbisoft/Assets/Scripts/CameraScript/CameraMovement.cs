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

            //transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(controllerYAccumulator,0,0),cameraRotationSmoothingSpeed);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(controllerYAccumulator, controllerXAccumulator, 0), cameraRotationSmoothingSpeed);
            transform.rotation = rotation;
            //playerBody.MoveRotation(Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z),cameraRotationSmoothingSpeed));

            //playerBody.MoveRotation(Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z));
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            controllerYAccumulator -= controllerY_PS;
            controllerXAccumulator += controllerX_PS;
            controllerYAccumulator = Mathf.Clamp(controllerYAccumulator, -90f, 90f);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(controllerYAccumulator,0,0),cameraRotationSmoothingSpeed);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(controllerXAccumulator, controllerYAccumulator, 0), cameraRotationSmoothingSpeed);
            transform.rotation = rotation;

            //playerBody.MoveRotation(Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z),cameraRotationSmoothingSpeed));
            // playerBody.MoveRotation(Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z));
            //transform.rotation = Quaternion.Slerp(playerBody.rotation, Quaternion.Euler(playerBody.rotation.x, controllerXAccumulator, playerBody.rotation.z), cameraRotationSmoothingSpeed);
        }
        else
        {
            mouseXAccumulator += mouseX;
            float previousMouseY = mouseYAccumulator;
            mouseYAccumulator -= mouseY;
            mouseYAccumulator = Mathf.Clamp(mouseYAccumulator, -90f, 90f);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(mouseYAccumulator, mouseXAccumulator, 0),cameraRotationSmoothingSpeed);
            transform.rotation = rotation;
            //playerBody.MoveRotation(Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(playerBody.rotation.x,mouseXAccumulator,playerBody.rotation.z),cameraRotationSmoothingSpeed));
            //transform.rotation = (Quaternion.Slerp(transform.rotation,Quaternion.Euler(transform.rotation.x,mouseXAccumulator, transform.rotation.z),cameraRotationSmoothingSpeed));
           
        }
    }
}
