using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CharacterControl : Serializable
{
    [Serializable]
    public class DTO
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public float RotationW { get; set; }
    }

    [SerializeField] private float playerMovementSpeed = 1f;
    [SerializeField] private GameController.Role _owner = GameController.Role.SecurityGuard;
    private Rigidbody playerBody;
    private Vector3 inputVector;
    private Vector3 gravityVector;
    
    private float keyboardHorizontal ;
    private float keyBoardVertical ;
    private float controllerHorizontal ;
    private float controllerVertical ;

    private NetworkController _networkController = null;
    
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
    }
    
    void Update()
    {
        if(_owner == _networkController.GetLocalRole())
        {
            keyboardHorizontal = Input.GetAxis("Horizontal");
            keyBoardVertical = Input.GetAxis("Vertical");
            controllerHorizontal = Input.GetAxis("LeftJoystickHorizontal");
            controllerVertical = Input.GetAxis("LeftJoystickVertical");

            Vector3 controllerInput = (transform.right * controllerHorizontal + transform.forward * controllerVertical) * (playerMovementSpeed);
            Vector3 keyboardInput = (playerBody.transform.right * keyboardHorizontal + playerBody.transform.forward * keyBoardVertical) * (playerMovementSpeed);

            inputVector = controllerInput + keyboardInput;

            if (inputVector.magnitude > playerMovementSpeed)
            {
                inputVector = Vector3.ClampMagnitude(inputVector, playerMovementSpeed);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_owner == _networkController.GetLocalRole())
        {
            playerBody.velocity = inputVector;
        }
    }

    public override void Deserialize(byte[] data)
    {

    }

    public override byte[] Serialize()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            bf.Serialize(memoryStream, new DTO()
            {
                PositionX = transform.transform.position.x,
                PositionY = transform.transform.position.y,
                PositionZ = transform.transform.position.z,
                RotationX = transform.transform.rotation.x,
                RotationY = transform.transform.rotation.y,
                RotationZ = transform.transform.rotation.z,
                RotationW = transform.transform.rotation.w,
            });

            return memoryStream.ToArray();
        }
    }
    public override void Smooth(byte[] oldData, byte[] newData, float lag, double lastTime, double currentTime)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var memStream = new MemoryStream())
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memStream.Write(newData, 0, newData.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                DTO dto = (DTO)bf.Deserialize(memStream);

                transform.position = Vector3.MoveTowards(transform.position, new Vector3(dto.PositionX, dto.PositionY, dto.PositionZ), Time.deltaTime * playerMovementSpeed);
            }
        }
    }
}
