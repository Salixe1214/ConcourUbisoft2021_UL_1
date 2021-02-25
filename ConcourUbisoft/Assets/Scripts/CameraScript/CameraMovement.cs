using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float mouseSensivityX =120;
    [SerializeField] private float mouseSensivityY =120;
    [SerializeField] private float mouseSnappinessX = 6;
    [SerializeField] private float mouseSnappinessY = 6;
    [SerializeField] private float controllerSensivityX=120;
    [SerializeField] private float controllerSensivityY=120;
    [SerializeField] private float controllerSnappinessX = 6;
    [SerializeField] private float controllerSnappinessY = 7;
    
    private Vector3 velocity = Vector3.zero;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float mouseYAccumulator = 0f;
    private float mouseXAccumulator = 0f;
    private float controllerYAccumulator = 0f;
    private float xRotationControllerPS = 0f;
    private float yRotationControllerPS = 0f;
    private float xRotationControllerXBO = 0f;
    private float yRotationControllerXBO = 0f;
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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivityX *Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivityY *Time.deltaTime;
        
        float controllerX_PS = Input.GetAxis("RightJoystickHorizontalPS")*controllerSensivityX*Time.deltaTime;
        float controllerY_PS = Input.GetAxis("RightJoystickVerticalPS")*controllerSensivityY*Time.deltaTime;
        float controllerX_XBO = Input.GetAxis("RightJoystickHorizontalXBO")*controllerSensivityX*Time.deltaTime;
        float controllerY_XBO = Input.GetAxis("RightJoystickVerticalXBO") * controllerSensivityY * Time.deltaTime;

        joysticks = Input.GetJoystickNames();

        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
            controllerYAccumulator -= controllerY_XBO;
               controllerYAccumulator = Mathf.Clamp(controllerYAccumulator, -90f, 90f);
               xRotationControllerXBO = Mathf.Lerp(xRotationControllerXBO, controllerYAccumulator, controllerSnappinessY * Time.deltaTime);
               transform.localRotation = Quaternion.Euler(xRotationControllerXBO,0, 0f);
               yRotationControllerXBO = Mathf.Lerp(yRotationControllerXBO, controllerX_XBO, controllerSnappinessX * Time.deltaTime);
              // playerBody.transform.Rotate(new Vector3(0,yRotationControllerXBO,0));
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            controllerYAccumulator -= controllerY_PS;
            controllerYAccumulator = Mathf.Clamp(controllerYAccumulator, -90f, 90f);
            xRotationControllerPS = Mathf.Lerp(xRotationControllerPS, controllerYAccumulator, controllerSnappinessY * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(xRotationControllerPS,0, 0f);
            yRotationControllerPS = Mathf.Lerp(yRotationControllerPS, controllerX_PS, controllerSnappinessX * Time.deltaTime);
           // playerBody.transform.Rotate(new Vector3(0,yRotationControllerPS,0));
        }
        else
        {
            mouseXAccumulator += mouseX;
            mouseYAccumulator -= mouseY;
            mouseYAccumulator = Mathf.Clamp(mouseYAccumulator, -90f, 90f);
            xRotation = Mathf.Lerp(xRotation, mouseYAccumulator, mouseSnappinessY * Time.deltaTime);
            //transform.localRotation = Quaternion.Euler(xRotation,0, 0f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(mouseYAccumulator,0,0),mouseSnappinessY*Time.deltaTime);
            
            //yRotation = Mathf.Lerp(yRotation, mouseXAccumulator, mouseSnappinessX * Time.deltaTime);
            //yRotation += yRotation;
            //playerBody.rotation= Quaternion.Euler(0,mouseXAccumulator,0);
            playerBody.rotation = Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(0,mouseXAccumulator,0),mouseSnappinessX*Time.deltaTime);
            //yRotation = mouseX;

        }
    }

    private void FixedUpdate()
    {
        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
            playerBody.transform.Rotate(new Vector3(0,yRotationControllerXBO,0));
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            playerBody.transform.Rotate(new Vector3(0,yRotationControllerPS,0));
        }
        else
        {
            //playerBody.transform.Rotate(new Vector3(0, yRotation, 0));
            //playerBody.rotation= Quaternion.Euler(0,yRotation,0);
            //playerBody.rotation= Quaternion.Euler(0,yRotation,0);
           // playerBody.rotation = Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(0,mouseXAccumulator,0),mouseSnappinessX*Time.deltaTime);
        }
    }
}
