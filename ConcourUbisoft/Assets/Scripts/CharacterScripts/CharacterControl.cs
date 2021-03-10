using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{

    [SerializeField] private float playerMovementSpeed = 1f;
    private Rigidbody playerBody;
    private Vector3 inputVector;
    private Vector3 gravityVector;
    
    private float keyboardHorizontal ;
    private float keyBoardVertical ;
    private float controllerHorizontal ;
    private float controllerVertical ;
    
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
    }
    
    void Update()
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

    private void FixedUpdate()
    {
        playerBody.velocity = inputVector;
    } 
}
