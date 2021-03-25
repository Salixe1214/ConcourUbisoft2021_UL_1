using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CharacterControl : MonoBehaviour, IPunObservable
{
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
    private PhotonView _photonView = null;

    private Vector3 newPosition = new Vector3();
    private Quaternion newQuartenion = new Quaternion();

    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _photonView = GetComponent<PhotonView>();
        transform.Rotate(Vector3.up, Mathf.Deg2Rad * 180);
        if (_networkController.GetLocalRole() == _owner)
        {
            _photonView.RequestOwnership();
        }
    }

    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
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
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.fixedDeltaTime * playerMovementSpeed);
            transform.rotation = newQuartenion;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            
            stream.SendNext(transform.position.x);
            stream.SendNext(transform.position.y);
            stream.SendNext(transform.position.z);
            stream.SendNext(transform.rotation.x);
            stream.SendNext(transform.rotation.y);
            stream.SendNext(transform.rotation.z);
            stream.SendNext(transform.rotation.w);
        }
        else
        {
            Vector3 newPostion = new Vector3((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
            if (Vector3.Distance(newPostion, transform.position) > 3)
            {
                newPosition = newPostion;
            }
            else
            {
                newPosition = newPostion;
               
            }
            newQuartenion = new Quaternion((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
        }
    }
}
