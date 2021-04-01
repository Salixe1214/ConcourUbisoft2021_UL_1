using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arm;
using UnityEngine;
using UnityEngine.Events;

public class PlayerNetwork : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameController.Role _playerRole;

    public GameController.Role PlayerRole { get => _playerRole; set => _playerRole = value; }
    public string Name { set; get; }
    public string Id { get { return _photonView.Owner.UserId; } }

    private PhotonView _photonView = null;
    private NetworkController _networkController = null;
    private GameController _gameController = null;

    #region Unity Callbacks

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Name = $"Player {(_photonView.Owner.IsMasterClient ? "1" : "2")}";
    }

    private void Start()
    {
        _networkController.InvokePlayerNetworkInstantiate();
    }

    private void OnEnable()
    {
        _gameController.OnLoadGameEvent += OnLoadGameEvent;
        _gameController.OnFinishLoadGameEvent += OnFinishLoadGameEvent;
        _gameController.OnFinishGameEvent += OnFinishGameEvent;
        _networkController.OnMasterChanged += _networkController_OnMasterChanged;
    }

    private void _networkController_OnMasterChanged()
    {
        Name = $"Player {(_photonView.Owner.IsMasterClient ? "1" : "2")}";
    }

    private void OnDisable()
    {
        _gameController.OnLoadGameEvent -= OnLoadGameEvent;
        _gameController.OnFinishLoadGameEvent -= OnFinishLoadGameEvent;
        _gameController.OnFinishGameEvent -= OnFinishGameEvent;
        _networkController.OnMasterChanged -= _networkController_OnMasterChanged;
    }

    #endregion
    #region Photon Callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)PlayerRole);
            stream.SendNext(Name);
        }
        else
        {
            this.PlayerRole = (GameController.Role)(int)stream.ReceiveNext();
            this.Name = (string)stream.ReceiveNext();
        }
    }

    #endregion
    #region Public Functions

    public bool IsMasterClient()
    {
        return _photonView.Owner.IsMasterClient;
    }

    public bool IsMine()
    {
        return _photonView.IsMine;
    }

    #endregion
    #region RPC Functions

    [PunRPC]
    private void StartGame()
    {
        if (!(_gameController.IsGameLoading || _gameController.IsGameStart))
        {
            _gameController.StartGame(_networkController.GetLocalRole());
        }
    }

    #endregion
    #region Private Functions

    private void OnLoadGameEvent()
    {
        _photonView.RPC("StartGame", RpcTarget.Others);
    }

    private void OnFinishLoadGameEvent()
    {
        
    }

    private void OnFinishGameEvent()
    {

    }

    #endregion
}