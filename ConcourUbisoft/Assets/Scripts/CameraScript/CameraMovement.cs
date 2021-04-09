using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Voice;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float mouseSensitivityX =120;
    [SerializeField] private float mouseSensitivityY =120;
    [SerializeField] private float controllerSensitivityX=120;
    [SerializeField] private float controllerSensitivityY=120;
    [SerializeField] private float cameraRotationSmoothingSpeed = 0.7f;
    
    private float mouseYAccumulator = 0f;
    private float mouseXAccumulator = 0f;
    private float controllerYAccumulator = 0f;
    private float controllerXAccumulator = 0f;
    
    private string[] joysticks;
    private Vector3 _cameraDifference = new Vector3();
    private CharacterControl _characterControl = null;
    private GameController _gameController = null;

    public bool invetedY { get; set; } = false;

    private void Awake()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        invetedY = _gameController.invertedY;
        _gameController.changeInverted.AddListener(changeInvertY);
        
        _characterControl = playerBody.GetComponent<CharacterControl>();

        mouseXAccumulator = transform.rotation.eulerAngles.y;
        controllerXAccumulator = transform.rotation.eulerAngles.y;
    }

    void Start()
    {
        joysticks = Input.GetJoystickNames();
        Cursor.lockState = CursorLockMode.Locked;

        _cameraDifference = transform.position - playerBody.transform.position;
    }



    //PS = playstation
    //XBO = xbox one
    void Update()
    {
        joysticks = Input.GetJoystickNames();
        if(!_gameController.IsGameMenuOpen)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerBody.transform.position + _cameraDifference, _characterControl.playerMovementSpeed * Time.deltaTime);

            RotateCamera();
        }
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") *mouseSensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
        if (invetedY)
            mouseY *= -1;
        
        float controllerX_PS = Mathf.Pow(Input.GetAxis("RightJoystickHorizontalPS")*controllerSensitivityX, 3);
        float controllerY_PS = Mathf.Pow(Input.GetAxis("RightJoystickVerticalPS")*controllerSensitivityY, 3);
        if (invetedY)
            controllerY_PS *= -1;
        
        float controllerX_XBO = Mathf.Pow(Input.GetAxis("RightJoystickHorizontalXBO")*controllerSensitivityX, 3);
        float controllerY_XBO = Mathf.Pow(Input.GetAxis("RightJoystickVerticalXBO") * controllerSensitivityY, 3);
        if (invetedY)
            controllerY_XBO *= -1;
        
        
        if ((joysticks.Contains("Controller (Xbox One For Windows)"))&& !_gameController.IsEndGameMenuOpen)
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
        else if (joysticks.Contains("Wireless Controller") && !_gameController.IsEndGameMenuOpen)
        {
            controllerYAccumulator -= controllerY_PS;
            controllerXAccumulator += controllerX_PS;
            controllerYAccumulator = Mathf.Clamp(controllerYAccumulator, -90f, 90f);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.Euler(controllerYAccumulator,0,0),cameraRotationSmoothingSpeed);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(controllerYAccumulator, controllerXAccumulator, 0), cameraRotationSmoothingSpeed);
            transform.rotation = rotation;

            //playerBody.MoveRotation(Quaternion.Slerp(playerBody.rotation,Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z),cameraRotationSmoothingSpeed));
            // playerBody.MoveRotation(Quaternion.Euler(playerBody.rotation.x,controllerXAccumulator,playerBody.rotation.z));
            //transform.rotation = Quaternion.Slerp(playerBody.rotation, Quaternion.Euler(playerBody.rotation.x, controllerXAccumulator, playerBody.rotation.z), cameraRotationSmoothingSpeed);
        }
        else if(!_gameController.IsEndGameMenuOpen)
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

    private void changeInvertY()
    {
        invetedY = _gameController.invertedY;
    }
}
