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
    public GameController.Role PlayerRole { get => _playerRole; set => _playerRole = value; }
    public string Name { set; get; }

    public string Id
    {
        get { return photonView.Owner.UserId; }
    }

    private new PhotonView photonView = null;
    private NetworkController networkController = null;
    private GameController gameController = null;

    private IEnumerable<NetworkSync> objectsToSync = new NetworkSync[0];
    [SerializeField] private GameController.Role _playerRole;

    #region Unity Callbacks

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Name = $"Player {(photonView.Owner.IsMasterClient ? "1" : "2")}";
        objectsToSync = GameObject.FindObjectsOfType<NetworkSync>().OrderBy(x => x.Id);
    }

    private void Start()
    {
        networkController.InvokePlayerNetworkInstantiate();
    }

    private void OnEnable()
    {
        gameController.OnLoadGameEvent += OnLoadGameEvent;
        gameController.OnFinishLoadGameEvent += OnFinishLoadGameEvent;
        gameController.OnFinishGameEvent += OnFinishGameEvent;
    }

    private void OnDisable()
    {
        gameController.OnLoadGameEvent -= OnLoadGameEvent;
        gameController.OnFinishLoadGameEvent -= OnFinishLoadGameEvent;
        gameController.OnFinishGameEvent -= OnFinishGameEvent;
    }

    private void Update()
    {
        foreach (NetworkSync syncObject in objectsToSync)
        {
            if (PlayerRole != syncObject.Owner)
            {
                syncObject.Smooth();
            }
        }
    }

    #endregion

    #region Photon Callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
        if (stream.IsWriting)
        {
            stream.SendNext((int)PlayerRole);
            stream.SendNext(Name);
            stream.SendNext((int)objectsToSync.Count(x => x.Owner == PlayerRole));
            foreach (NetworkSync syncObject in objectsToSync)
            {
                if (PlayerRole == syncObject.Owner)
                {
                    stream.SendNext((int)syncObject.Id);
                    stream.SendNext(syncObject.Serialize());
                }
            }
        }
        else
        {
            this.PlayerRole = (GameController.Role)(int)stream.ReceiveNext();
            this.Name = (string)stream.ReceiveNext();

            int serializeCount = (int)stream.ReceiveNext();

            for (int i = 0; i < serializeCount; ++i)
            {
                int syncId = (int)stream.ReceiveNext();
                NetworkSync sync = objectsToSync.Where(x => x.Id == syncId).FirstOrDefault();

                byte[] data = (byte[])stream.ReceiveNext();
                if (sync != null)
                {
                    sync.Deserialize(data, lag, info.SentServerTime);
                }
                
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

    [PunRPC]
    private void UnlockDoor(object[] parameters)
    {
        DoorsScript doorsScript = FindObjectsOfType<DoorsScript>().Where(x => x.DoorId == (int)parameters[0])
            .FirstOrDefault();
        if (doorsScript != null)
        {
            doorsScript.UnlockDoor();
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
        objectsToSync = GameObject.FindObjectsOfType<NetworkSync>().OrderBy(x => x.Id);

        if (IsMine())
        {
            DoorsScript[] doorsScripts = FindObjectsOfType<DoorsScript>();
            foreach (DoorsScript door in doorsScripts)
            {
                door.OnDoorUnlockEvent += OnDoorUnlockEvent;
            }
        }
    }

    private void OnDoorUnlockEvent(DoorsScript doorsScript)
    {
        object[] parameters = new object[] { doorsScript.DoorId };
        photonView.RPC("UnlockDoor", RpcTarget.Others, parameters as object);
    }

    private void OnFinishGameEvent()
    {
        DoorsScript[] doorsScripts = FindObjectsOfType<DoorsScript>();
        foreach (DoorsScript door in doorsScripts)
        {
            door.OnDoorUnlockEvent -= OnDoorUnlockEvent;
        }
    }

    #endregion
}