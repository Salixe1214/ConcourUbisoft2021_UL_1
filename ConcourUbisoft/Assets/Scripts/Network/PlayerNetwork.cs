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
    public GameController.Role PlayerRole { get; set; }
    public string Name { set; get; }

    public string Id
    {
        get { return photonView.Owner.UserId; }
    }

    private new PhotonView photonView = null;
    private NetworkController networkController = null;
    private GameController gameController = null;
    private GameObject playerA = null;
    private ArmController[] Arms;

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
        gameController.OnFinishGameEvent += OnFinishGameEvent;
    }

    private void OnDisable()
    {
        gameController.OnLoadGameEvent -= OnLoadGameEvent;
        gameController.OnFinishLoadGameEvent -= OnFinishLoadGameEvent;
        gameController.OnFinishGameEvent -= OnFinishGameEvent;
    }

    #endregion

    #region Photon Callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int) PlayerRole);
            stream.SendNext(Name);

            if (gameController.IsGameStart && PlayerRole == GameController.Role.SecurityGuard)
            {
                stream.SendNext(playerA.transform.position);
                stream.SendNext(playerA.transform.rotation);
            }
            else if (gameController.IsGameStart && PlayerRole == GameController.Role.Technician)
            {
                for (int i = 0; i < Arms.Length; i++)
                {
                    stream.SendNext(Arms[i].transform.position);
                    stream.SendNext(Arms[i].transform.rotation);
                }
            }
        }
        else
        {
            this.PlayerRole = (GameController.Role) (int) stream.ReceiveNext();
            this.Name = (string) stream.ReceiveNext();

            if (gameController.IsGameStart && PlayerRole == GameController.Role.SecurityGuard)
            {
                playerA.transform.position = (Vector3) stream.ReceiveNext();
                playerA.transform.rotation = (Quaternion) stream.ReceiveNext();
            }
            else if (gameController.IsGameStart && PlayerRole == GameController.Role.Technician)
            {
                for (int i = 0; i < Arms.Length; i++)
                {
                    Arms[i].transform.position = (Vector3) stream.ReceiveNext();
                    Arms[i].transform.rotation = (Quaternion) stream.ReceiveNext();
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
        DoorsScript doorsScript = FindObjectsOfType<DoorsScript>().Where(x => x.DoorId == (int) parameters[0])
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
        playerA = GameObject.FindGameObjectWithTag("PlayerGuard");
        Arms = new ArmController[1];
        Arms[0] = GameObject.FindGameObjectWithTag("Arm1").GetComponent<ArmController>();
        //Arms[1] = GameObject.FindGameObjectWithTag("Arm2").GetComponent<ArmController>();

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
        object[] parameters = new object[] {doorsScript.DoorId};
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