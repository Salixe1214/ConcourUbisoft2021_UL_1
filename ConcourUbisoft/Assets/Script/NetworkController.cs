using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject RoomListElementPrefab = null;
    [SerializeField] private GameObject RoomListPanel = null;

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
        foreach (Transform child in RoomListPanel.transform) {
            Destroy(child.gameObject);
        }

        int i = 0;
        roomList.ForEach((roomInfo) => {
            GameObject roomListElement = Instantiate(RoomListElementPrefab, RoomListPanel.transform);
            roomListElement.transform.localPosition = new Vector3(RoomListPanel.GetComponent<RectTransform>().rect.x, RoomListPanel.GetComponent<RectTransform>().rect.y + RoomListPanel.GetComponent<RectTransform>().rect.height - i * 25);
            roomListElement.transform.Find("RoomName").GetComponent<Text>().text = roomInfo.Name;
            roomListElement.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(() => { PhotonNetwork.JoinRoom(roomInfo.Name); });
            i++;
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
