using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class NetworkController : MonoBehaviourPunCallbacks
{
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

    public delegate void OnJoinedRoomHandler();
    public event OnJoinedRoomHandler OnJoinedRoomEvent;

    public delegate void OnPlayerJoinRoomHandler();
    public event OnPlayerJoinRoomHandler OnPlayerJoin;

    public delegate void OnPlayerLeftRoomHandler();
    public event OnPlayerLeftRoomHandler OnPlayerLeft;

    public delegate void OnLeftRoomHandler();
    public event OnLeftRoomHandler OnLeftRoomEvent;

    public delegate void OnDisconnectHandler();
    public event OnDisconnectHandler OnDisconnectEvent;

    public delegate void OnPlayerNetworkInstantiateHandler();
    public event OnPlayerNetworkInstantiateHandler OnPlayerObjectCreate;
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
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        OnRoomListUpdateEvent?.Invoke(roomList.Select(roomInfo => new RoomInformation() { RoomName = roomInfo.Name, PlayerCount = roomInfo.PlayerCount, Action = () => { PhotonNetwork.JoinRoom(roomInfo.Name); } }));
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity, 0);
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
        OnPlayerJoin?.Invoke();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerLeft?.Invoke();
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
        return GameObject.FindGameObjectsWithTag("PlayerNetwork").Select(x => x.GetComponent<PlayerNetwork>()).Where(x => x.IsMine()).First().PlayerRole;
    }
    #endregion
}
