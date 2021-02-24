using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{

    [SerializeField] private float playerMovementSpeed = 1f; 
    private Rigidbody playerBody;
    private Vector3 inputVector;
    
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        float keyboardHorizontal = Input.GetAxis("Horizontal");
        float keyBoardVertical = Input.GetAxis("Vertical");
        float controllerHorizontal = Input.GetAxis("LeftJoystickHorizontal");
        float controllerVertical = Input.GetAxis("LeftJoystickVertical");
        
        Vector3 gravityVector = new Vector3(0,playerBody.velocity.y,0);
        Vector3 controllerInput = (transform.right * controllerHorizontal + transform.forward *controllerVertical)*playerMovementSpeed;
        Vector3 keyboardInput = (transform.right * keyboardHorizontal + transform.forward *keyBoardVertical)*playerMovementSpeed;
        
        inputVector = controllerInput + keyboardInput + gravityVector;
    }

    private void FixedUpdate()
    {
        if (inputVector.magnitude > playerMovementSpeed)
        {
            inputVector = Vector3.ClampMagnitude(inputVector,playerMovementSpeed);
        }
        playerBody.velocity = inputVector;
    }
}
