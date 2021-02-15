using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviourPun, IPunObservable
{
    public GameController.Role PlayerRole { get; set; }
    public string Name { set; get; }
    public string Id { get { return photonView.Owner.UserId; } }

    private new PhotonView photonView = null;
    private NetworkController networkController = null;
    private GameController gameController = null;
    private GameObject playerA = null;

    #region Unity Callbacks
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        
        Name = $"Player {(photonView.Owner.IsMasterClient ? "1" : "2")}";
    }
    private void Start()
    {
        networkController.InvokePlayerNetworkInstantiate();
    }
    private void OnEnable()
    {
        gameController.OnLoadGameEvent += OnLoadGameEvent;
        gameController.OnFinishLoadGameEvent += OnFinishLoadGameEvent;
    }
    private void OnDisable()
    {
        gameController.OnLoadGameEvent -= OnLoadGameEvent;
        gameController.OnFinishLoadGameEvent -= OnFinishLoadGameEvent;
    }


    #endregion
    #region Photon Callbacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)PlayerRole);
            stream.SendNext(Name);

            if(gameController.IsGameStart && PlayerRole == GameController.Role.SecurityGuard)
            {
                stream.SendNext(playerA.transform.position);
                stream.SendNext(playerA.transform.rotation);
            }
        }
        else
        {
            this.PlayerRole = (GameController.Role)(int)stream.ReceiveNext();
            this.Name = (string)stream.ReceiveNext();

            if (gameController.IsGameStart && PlayerRole == GameController.Role.SecurityGuard)
            {
                playerA.transform.position = (Vector3)stream.ReceiveNext();
                playerA.transform.rotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
    #endregion
    #region Public Functions
    public bool IsMasterClient()
    {
        return photonView.Owner.IsMasterClient;
    }
    public bool IsMine()
    {
        return photonView.IsMine;
    }
    #endregion
    #region RPC Functions
    [PunRPC]
    private void StartGame()
    {
        if (!(gameController.IsGameLoading || gameController.IsGameStart))
        {
            gameController.StartGame(networkController.GetLocalRole());
        }
    }
    #endregion
    #region Private Functions
    private void OnLoadGameEvent()
    {
        photonView.RPC("StartGame", RpcTarget.Others);
    }
    private void OnFinishLoadGameEvent()
    {
        playerA = GameObject.FindGameObjectWithTag("Player");
    }
    #endregion
}
