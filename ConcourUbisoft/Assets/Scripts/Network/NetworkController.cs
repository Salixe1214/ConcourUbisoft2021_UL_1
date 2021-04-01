using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Photon.Voice.PUN;
using Photon.Voice.Unity.UtilityScripts;
using UnityEditor;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool QuickSetup = false;
    [SerializeField] private List<GameObject> networkObjects = null;

    public float photonPing = 0;
    public int photonSendRate = 30;
    public int photonSendRateSerialize = 30;

    private GameController _gameController = null;
    private PlayerNetwork ownNetworkPlayer = null;

    #region Unity Callbacks
    private void Awake()
    {
        PhotonNetwork.SendRate = photonSendRate;
        PhotonNetwork.SerializationRate = photonSendRateSerialize;
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    private void Start()
    {
        if(QuickSetup)
        {
            JoinLobby();
        }
    }
    private void Update()
    {
        photonPing = PhotonNetwork.GetPing();
    }
    #endregion
    #region Events
    public delegate void OnJoinedLobbyHandler();
    public event OnJoinedLobbyHandler OnJoinedLobbyEvent;

    public class RoomInformation
    {
        public string RoomName { get; set; }
        public int PlayerCount { get; set; }
        public UnityAction Action { get; set; }
    }
    public delegate void OnRoomListUpdateHandler(IEnumerable<RoomInformation> roomInformations);
    public event OnRoomListUpdateHandler OnRoomListUpdateEvent;

    public delegate void OnNetworkErrorHandler(string errorTitle, string errorMessage);
    public event OnNetworkErrorHandler OnNetworkErrorEvent;

    public event Action OnJoinedRoomEvent;
    public event Action OnPlayerJoin;
    public event Action OnPlayerLeftEvent;
    public event Action OnLeftRoomEvent;
    public event Action OnDisconnectEvent;
    public event Action OnPlayerObjectCreate;
    public event Action OnMasterChanged;

    #endregion
    #region Photon Callbacks
    public override void OnDisconnected(DisconnectCause cause)
    {
        OnDisconnectEvent?.Invoke();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    public override void OnJoinedLobby()
    {
        OnJoinedLobbyEvent?.Invoke();
        if (QuickSetup)
        {
            PhotonNetwork.JoinOrCreateRoom("DEFAULT_TEST", new RoomOptions() {IsVisible = false, MaxPlayers = 2 }, null);
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        OnRoomListUpdateEvent?.Invoke(roomList.Select(roomInfo => new RoomInformation() { RoomName = roomInfo.Name, PlayerCount = roomInfo.PlayerCount, Action = () => { PhotonNetwork.JoinRoom(roomInfo.Name); } }));
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        GameObject playerNetwork = PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity, 0);
        playerNetwork.GetComponent<ConnectAndJoin>().RoomName = PhotonNetwork.CurrentRoom.Name;
        playerNetwork.GetComponent<ConnectAndJoin>().ConnectNow();
        if (QuickSetup)
        {
            GameController.Role role = PhotonNetwork.IsMasterClient ? GameController.Role.SecurityGuard : GameController.Role.Technician;
            playerNetwork.GetComponent<PlayerNetwork>().PlayerRole = role;

            GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            gameController.GameRole = role;
            gameController.IsGameStart = true;
        }

        OnJoinedRoomEvent?.Invoke();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        OnNetworkErrorEvent?.Invoke("An error occured while joining a room.", message);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        OnNetworkErrorEvent?.Invoke("An error occured while creating a room.", message);
    }
    public override void OnLeftRoom()
    {
        OnLeftRoomEvent?.Invoke();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (QuickSetup && PhotonNetwork.IsMasterClient)
        {

        }
        OnPlayerJoin?.Invoke();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_gameController.IsGameStart) {
            OnNetworkErrorEvent?.Invoke("A player left the game.", "A player left the game while the game was in progress. ");
        }
        OnPlayerLeftEvent?.Invoke();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        OnMasterChanged?.Invoke();
    }
    #endregion
    #region Public Functions
    public void JoinLobby()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void LeaveLobby()
    {
        PhotonNetwork.Disconnect();
    }
    public void CreateRoom(string roomName, bool privateGame)
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsVisible = !privateGame, PublishUserId = true });
    }
    public void JoinRoom(string roomName)
    {
        if (roomName != "")
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            OnNetworkErrorEvent?.Invoke("An error occured while joining a room.", "You must specify an Id to connect to room.");
        }
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }
    public void KickPlayer(string userId)
    {
        PhotonNetwork.CloseConnection(PhotonNetwork.PlayerList.Where(x => x.UserId == userId).First());
    }
    public void InvokePlayerNetworkInstantiate()
    {
        OnPlayerObjectCreate?.Invoke();
    }
    public GameController.Role GetLocalRole()
    {
        IEnumerable<PlayerNetwork> playerNetworks = GameObject.FindGameObjectsWithTag("PlayerNetwork").Select(x => x.GetComponent<PlayerNetwork>()).Where(x => x.IsMine());
        return playerNetworks.Count() > 0 ? playerNetworks.First().PlayerRole : GameController.Role.None;
    }
    public GameController.Role GetDistantRole()
    {
        IEnumerable<PlayerNetwork> playerNetworks = GameObject.FindGameObjectsWithTag("PlayerNetwork").Select(x => x.GetComponent<PlayerNetwork>()).Where(x => !x.IsMine());
        return playerNetworks.Count() > 0 ? playerNetworks.First().PlayerRole : GameController.Role.None;
    }

    public int GetNumberOfPlayer()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public GameObject GetPrefab(string name)
    {
        return networkObjects.Where(x => x.name == name).FirstOrDefault();
    }
    public PlayerNetwork GetOwnNetworkPlayer()
    {
        if (ownNetworkPlayer == null) {
            ownNetworkPlayer = GameObject.FindObjectsOfType<PlayerNetwork>().Where(x => x.PlayerRole == GetLocalRole()).FirstOrDefault();
        }

        return ownNetworkPlayer;
    }

    #endregion
}
