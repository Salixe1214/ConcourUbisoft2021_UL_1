using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{

    [SerializeField] private float playerMovementSpeed = 1f;
    [SerializeField] private float inputSmoothSpeed = 0;
    private Rigidbody playerBody;
    private Vector3 inputVector;
    private Vector3 smoothInputVector;
    private Vector3 gravityVector;
    
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
        
        gravityVector = new Vector3(0,playerBody.velocity.y,0);
        Vector3 controllerInput = (transform.right * controllerHorizontal + transform.forward *controllerVertical)*(playerMovementSpeed);
        Vector3 keyboardInput = (transform.right * keyboardHorizontal + transform.forward *keyBoardVertical)*(playerMovementSpeed);
        
        inputVector = controllerInput + keyboardInput;
        smoothInputVector =  Vector3.Lerp(smoothInputVector, inputVector, inputSmoothSpeed * Time.deltaTime);

        if (smoothInputVector.magnitude > playerMovementSpeed)
        {
            smoothInputVector = Vector3.ClampMagnitude(smoothInputVector,playerMovementSpeed);
        }
    }

    private void FixedUpdate()
    {
        playerBody.velocity = smoothInputVector + gravityVector;
    }
}
