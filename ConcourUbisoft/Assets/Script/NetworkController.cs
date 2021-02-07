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
    public class RoomInformation
    {
        public string RoomName { get; set; }
        public int PlayerCount { get; set; }
        public UnityAction Action { get; set; }
    }

    public class PlayerInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public bool RoomOwner { get; set; }
    }

    [SerializeField] private GameObject LobbyMenu = null;
    [SerializeField] private GameObject RoomMenu = null;
    [SerializeField] private GameObject ErrorPanelErrorPrefab = null;
    [SerializeField] private GameObject Canvas = null;

    public delegate void OnNumberPlayerChangeRoomHandler();
    public event OnNumberPlayerChangeRoomHandler OnPlayerJoin;
    public event OnNumberPlayerChangeRoomHandler OnPlayerLeft;

    public delegate void OnLobbyRoomListUpdateHandler(IEnumerable<RoomInformation> roomInformations);
    public event OnLobbyRoomListUpdateHandler OnLobbyRoomListUpdate;

    public delegate void OnPlayerObjectCreateHandler();
    public event OnPlayerObjectCreateHandler OnPlayerObjectCreate;
    
    // For Voice Chat
    [SerializeField] private NetworkVoiceManager networkVoiceManger;

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        OnLobbyRoomListUpdate?.Invoke(roomList.Select(roomInfo => new RoomInformation() { RoomName = roomInfo.Name, PlayerCount = roomInfo.PlayerCount, Action = () => { PhotonNetwork.JoinRoom(roomInfo.Name); } }));
    }

    public void CreateRoom(string roomName, bool privateGame) {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsVisible = !privateGame, PublishUserId = true });
    }

    public void JoinRoom(string roomName) {
        if (roomName != "") {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            GameObject errorPanelError = Instantiate(ErrorPanelErrorPrefab, Canvas.transform);
            ErrorPromptController errorPromptController = errorPanelError.GetComponent<ErrorPromptController>();
            errorPromptController.ErrorTitle = "An error occured while joining a room.";
            errorPromptController.ErrorMessage = "You must specify an Id to connect to room.";
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        var newPLayer = PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity, 0);
        networkVoiceManger.RemoteVoiceParent = newPLayer.transform;
        RoomMenu.SetActive(true);
        LobbyMenu.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnPlayerJoin?.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerLeft?.Invoke();
    }

    public bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public void KickPlayer(string userId)
    {
        Debug.Log("KickPlayer");
        PhotonNetwork.CloseConnection(PhotonNetwork.PlayerList.Where(x => x.UserId == userId).First());
    }

    public void InvokePlayerObjectCreate()
    {
        OnPlayerObjectCreate?.Invoke();
    }

    public override void OnLeftRoom()
    {
        LobbyMenu.SetActive(true);
        RoomMenu.SetActive(false);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        GameObject errorPanelError = Instantiate(ErrorPanelErrorPrefab, Canvas.transform);
        ErrorPromptController errorPromptController = errorPanelError.GetComponent<ErrorPromptController>();
        errorPromptController.ErrorTitle = "An error occured while joining a room.";
        errorPromptController.ErrorMessage = message;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        GameObject errorPanelError = Instantiate(ErrorPanelErrorPrefab, Canvas.transform);
        ErrorPromptController errorPromptController = errorPanelError.GetComponent<ErrorPromptController>();
        errorPromptController.ErrorTitle = "An error occured while creating a room.";
        errorPromptController.ErrorMessage = message;
    }
}
