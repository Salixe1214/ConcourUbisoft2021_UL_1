using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private LobbyMenu LobbyMenu = null;

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
        LobbyMenu.ClearElements();

        roomList.ForEach((roomInfo) => {
            LobbyMenu.AddElement(roomInfo.Name, roomInfo.PlayerCount, () => { PhotonNetwork.JoinRoom(roomInfo.Name); });
        });
    }

    public void CreateRoom(string roomName, bool privateGame) {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsVisible = !privateGame });
    }

    public void JoinRoom(string roomName) {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
    }
}
