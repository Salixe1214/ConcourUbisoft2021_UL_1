using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Inputs;
using UnityEngine;

public class CharacterControl : MonoBehaviour, IPunObservable
{
    [SerializeField] public float playerMovementSpeed = 1f;
    [SerializeField] private GameController.Role _owner = GameController.Role.SecurityGuard;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private new Camera _camera = null;
    [SerializeField] private Transform _mesh;

    private Rigidbody playerBody;
    private Vector3 inputVector;
    private Vector3 gravityVector;
    
    private float keyboardHorizontal ;
    private float keyBoardVertical ;
    private float controllerHorizontal ;
    private float controllerVertical;
    private InputManager _inputManager;
    private Controller _currentController;

    private NetworkController _networkController = null;
    private PhotonView _photonView = null;

    private Vector3 newPosition = new Vector3();
    private Quaternion newQuartenion = new Quaternion();
    private bool _isMoving = false;
    private float _horizontalMovement = 0.0f;
    private float _verticalMovement = 0.0f;
    private GameController _gameController = null;
    

    [SerializeField] private DoorController door3;

    private void Awake()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        newPosition = transform.position;
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _inputManager = GameObject.FindWithTag("InputManager").GetComponent<InputManager>();
        _photonView = GetComponent<PhotonView>();
        transform.Rotate(Vector3.up, Mathf.Deg2Rad * 180);
        SimpleButton[] buttons = GameObject.FindObjectsOfType<SimpleButton>();
        foreach (SimpleButton button in buttons)
        {
            button.BeforeActions.AddListener(() => _photonView.RPC("SetAnimatorPressButton", RpcTarget.All, new object[] { true } as object));
            button.AfterActions.AddListener(() => _photonView.RPC("SetAnimatorPressButton", RpcTarget.All, new object[] { false } as object));
        }
    }

    private void OnEnable()
    {
        _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
    }

    private void OnDisable()
    {
        _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
    }

    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(7,3);
        _currentController = InputManager.GetController();
    }
    
    void Update()
    {
        if (_networkController.GetLocalRole() == _owner && !_photonView.IsMine)
        {
            _photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

        if (_owner == _networkController.GetLocalRole())
        {
            keyboardHorizontal = Input.GetAxis("Horizontal");
            keyBoardVertical = Input.GetAxis("Vertical");
            controllerHorizontal = Input.GetAxis("LeftJoystickHorizontal");
            controllerVertical = Input.GetAxis("LeftJoystickVertical");

            if(_gameController.IsGameMenuOpen || _gameController.IsEndGameMenuOpen)
            {
                _horizontalMovement = 0;
                _verticalMovement = 0;
                inputVector = Vector3.zero;
            }
            else
            {
                Vector3 controllerInput = Vector3.zero;
                Vector3 keyboardInput = Vector3.zero;
                
                if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
                {
                    controllerInput = (_camera.transform.right * controllerHorizontal + Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized * controllerVertical);
                }
                else if (_currentController == Controller.Other)
                {
                    keyboardInput = (_camera.transform.right * keyboardHorizontal + Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized * keyBoardVertical);
                }
                
                _horizontalMovement = Mathf.Clamp(Input.GetAxis("Horizontal") + Input.GetAxis("LeftJoystickHorizontal"), -1, 1);
                _verticalMovement = Mathf.Clamp(Input.GetAxis("Vertical") + Input.GetAxis("LeftJoystickVertical"), -1, 1);

                inputVector = (controllerInput + keyboardInput).normalized * (playerMovementSpeed);

                if(_horizontalMovement == 0 && _verticalMovement == 0)
                {
                    inputVector = Vector3.zero;
                }

                _mesh.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized, Vector3.up);
            }

            _isMoving = inputVector != Vector3.zero;
        }

        _animator.SetBool("IsMoving",_isMoving);
        _animator.SetFloat("VerticalMovement", _verticalMovement);
        _animator.SetFloat("HorizontalMovement", _horizontalMovement);
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
            _mesh.rotation = newQuartenion;
        }
    }

    [PunRPC]
    private void SetAnimatorPressButton(object[] parameters)
    {
        Debug.Log($"Set Press {(bool)parameters[0]}");
        if((bool)parameters[0])
        {
            _animator.SetBool("PressingButton", true);
        }
        else
        {
            StartCoroutine(TurnOffPressButton());
        }
    }

    IEnumerator TurnOffPressButton()
    {
        yield return 0;
        _animator.SetBool("PressingButton", false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isMoving);
            stream.SendNext(_verticalMovement);
            stream.SendNext(_horizontalMovement);
            stream.SendNext(transform.position.x);
            stream.SendNext(transform.position.y);
            stream.SendNext(transform.position.z);
            stream.SendNext(_mesh.rotation.x);
            stream.SendNext(_mesh.rotation.y);
            stream.SendNext(_mesh.rotation.z);
            stream.SendNext(_mesh.rotation.w);
        }
        else
        {
            _isMoving = (bool)stream.ReceiveNext();
            _verticalMovement = (float)stream.ReceiveNext();
            _horizontalMovement = (float)stream.ReceiveNext();
            Vector3 newPosition = new Vector3((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
            if (Vector3.Distance(newPosition, transform.position) > 3)
            {
                transform.position = newPosition;
            }
            else
            {
                this.newPosition = newPosition;
            }
            newQuartenion = new Quaternion((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndZoneCollider"))
        {
            door3.CloseDoor();
        }
    }

    private void OnControllerTypeChanged()
    {
        _currentController = InputManager.GetController();
    }

}
