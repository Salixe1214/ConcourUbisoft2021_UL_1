using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Voice;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float mouseSensivityX =120;
    [SerializeField] private float mouseSensivityY =120;
    [SerializeField] private float controllerSensivityX=120;
    [SerializeField] private float controllerSensivityY=120;
    [SerializeField] private float cameraRotationSmoothingSpeed = 0.7f;
    
    [SerializeField] private float playerMovementSpeed = 1f;
    private Vector3 inputVector;
    
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
    
    private float keyboardHorizontal ;
    private float keyBoardVertical ;
    private float controllerHorizontal ;
    private float controllerVertical ;
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
        SetPlayerMovement();
    }

    private void FixedUpdate()
    {
        playerBody.velocity = inputVector;
    }

    private void SetPlayerMovement()
    {
        keyboardHorizontal = Input.GetAxis("Horizontal");
        keyBoardVertical = Input.GetAxis("Vertical");
        controllerHorizontal = Input.GetAxis("LeftJoystickHorizontal");
        controllerVertical = Input.GetAxis("LeftJoystickVertical");
        
        Vector3 controllerInput = (transform.right * controllerHorizontal + transform.forward *controllerVertical)*(playerMovementSpeed);
        Vector3 keyboardInput = (playerBody.transform.right * keyboardHorizontal + playerBody.transform.forward *keyBoardVertical)*(playerMovementSpeed);
        
        inputVector = controllerInput + keyboardInput;
        
        if (inputVector.magnitude > playerMovementSpeed)
        {
            inputVector = Vector3.ClampMagnitude(inputVector,playerMovementSpeed);
        }
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivityX *Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivityY *Time.deltaTime;
        
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
            mouseYAccumulator -= mouseY;
            mouseYAccumulator = Mathf.Clamp(mouseYAccumulator, -90f, 90f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(mouseYAccumulator,0,0),cameraRotationSmoothingSpeed);
            //transform.localRotation = Quaternion.Euler(mouseYAccumulator, 0, 0);
            playerBody.MoveRotation(Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(playerBody.rotation.x,mouseXAccumulator,playerBody.rotation.z),cameraRotationSmoothingSpeed));
           // playerBody.MoveRotation(Quaternion.Euler(playerBody.rotation.x,mouseXAccumulator,playerBody.rotation.z));
        }
    }
}
