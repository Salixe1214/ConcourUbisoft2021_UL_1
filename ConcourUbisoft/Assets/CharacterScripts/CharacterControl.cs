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
        inputVector = new Vector3(Input.GetAxis("Horizontal") * playerMovementSpeed,playerBody.velocity.y,Input.GetAxis("Vertical")*playerMovementSpeed);
        playerBody.velocity = inputVector;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            transform.LookAt(playerBody.position + new Vector3(inputVector.x,0,inputVector.z));
        }
    }

    private void FixedUpdate()
    {
        
    }
}
