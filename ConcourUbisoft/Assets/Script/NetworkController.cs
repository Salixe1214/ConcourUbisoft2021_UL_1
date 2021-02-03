using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private LobbyPanel RoomListPanel = null;

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
        RoomListPanel.ClearElements();

        roomList.ForEach((roomInfo) => {
            RoomListPanel.AddElement(roomInfo.Name, () => { PhotonNetwork.JoinRoom(roomInfo.Name); });
        });
    }

    public void CreateRandomRoom() {
        PhotonNetwork.CreateRoom(Random.Range(0,100).ToString());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
    }
}
