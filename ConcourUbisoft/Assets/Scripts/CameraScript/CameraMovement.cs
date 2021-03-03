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
    [SerializeField] private float controllerSensivityX=120;
    [SerializeField] private float controllerSensivityY=120;
    [SerializeField] private float scrollSpeed = 0.1f;
    [SerializeField] private float cameraRotationSmoothingSpeed = 0.7f;
    
    private Vector3 velocity;
    private float smoothedScrollSpeed;
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
        velocity = Vector3.zero;
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
            controllerXAccumulator += controllerX_XBO;
            controllerYAccumulator = Mathf.Clamp(controllerYAccumulator, -90f, 90f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(controllerYAccumulator,controllerXAccumulator,0),cameraRotationSmoothingSpeed);
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            controllerYAccumulator -= controllerY_PS;
            controllerXAccumulator += controllerX_PS;
            controllerYAccumulator = Mathf.Clamp(controllerYAccumulator, -90f, 90f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(controllerYAccumulator,controllerXAccumulator,0),cameraRotationSmoothingSpeed);
        }
        else
        {
            mouseXAccumulator += mouseX;
            mouseYAccumulator -= mouseY;
            mouseYAccumulator = Mathf.Clamp(mouseYAccumulator, -90f, 90f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(mouseYAccumulator,mouseXAccumulator,0),cameraRotationSmoothingSpeed);
        }
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(playerBody.position.x, transform.position.y, playerBody.position.z),ref velocity, scrollSpeed);
    }

    private void FixedUpdate()
    {
        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
            playerBody.MoveRotation(Quaternion.Euler(0,controllerXAccumulator,0));
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            playerBody.MoveRotation(Quaternion.Euler(0,controllerXAccumulator,0));
        }
        else
        {
            playerBody.MoveRotation(Quaternion.Euler(0, mouseXAccumulator, 0));
        }
    }
}
