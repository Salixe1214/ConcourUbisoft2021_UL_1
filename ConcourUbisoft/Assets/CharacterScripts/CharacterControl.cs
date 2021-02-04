using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float playerMovementSpeed = 1f; 
    private Rigidbody playerBody;
    private Vector3 inputVector;
    
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 controllerInputVector = new Vector3(Input.GetAxis("LeftJoystickHorizontal")*playerMovementSpeed,0,Input.GetAxis("LeftJoystickVertical")*playerMovementSpeed);
        Vector3 keyboardInputVector = new Vector3(Input.GetAxis("Horizontal") * playerMovementSpeed,0,Input.GetAxis("Vertical")*playerMovementSpeed);
        Vector3 gravityVector = new Vector3(0,playerBody.velocity.y,0);
        //inputVector = new Vector3(Input.GetAxis("Horizontal") * playerMovementSpeed,playerBody.velocity.y,Input.GetAxis("Vertical")*playerMovementSpeed);
        inputVector = controllerInputVector + keyboardInputVector + gravityVector;
        playerBody.velocity = inputVector;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Mathf.Abs(Input.GetAxis("LeftJoystickHorizontal")) > 0.1 || Mathf.Abs(Input.GetAxis("LeftJoystickVertical")) > 0.1)
        {
            transform.LookAt(playerBody.position + new Vector3(inputVector.x,0,inputVector.z));
        }
    }

    private void FixedUpdate()
    {
        
    }
}
