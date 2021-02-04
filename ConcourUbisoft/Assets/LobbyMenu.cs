using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject RoomListElementPrefab = null;
    [SerializeField] private GameObject ContentPanel = null;
    [SerializeField] private GameObject LobbyPanel = null;
    [SerializeField] private GameObject CreateRoomPanel = null;
    [SerializeField] private GameObject TogglePrivate = null;
    [SerializeField] private GameObject JoinRoomPanel = null;

    private NetworkController networkController = null;

    private void Awake()
    {
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
    }

    private void OnEnable()
    {
        networkController.OnLobbyRoomListUpdate += NetworkController_OnLobbyRoomListUpdate;
    }

    private void OnDisable()
    {
        networkController.OnLobbyRoomListUpdate -= NetworkController_OnLobbyRoomListUpdate;
    }

    private void NetworkController_OnLobbyRoomListUpdate(IEnumerable<NetworkController.RoomInformation> roomInformations)
    {
        ClearElements();

        roomInformations.All((roomInfo) => {
            AddElement(roomInfo.RoomName, roomInfo.PlayerCount, () => { PhotonNetwork.JoinRoom(roomInfo.RoomName); } ); return true;
        });
    }

    private void AddElement(string name, int playerCount, UnityAction action) {
        GameObject roomListElement = Instantiate(RoomListElementPrefab, ContentPanel.transform);
        roomListElement.transform.Find("RoomName").GetComponent<Text>().text = name;
        roomListElement.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(action);
        roomListElement.transform.Find("PlayerCount").GetComponent<Text>().text = $"{playerCount.ToString()}/2";
    }

    private void ClearElements()
    {
        foreach (Transform child in ContentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateRoom() {
        networkController.CreateRoom(CreateRoomPanel.transform.Find("RoomNameInputField").GetComponent<InputField>().text, TogglePrivate.GetComponent<Toggle>().isOn);
        CreateRoomPanel.SetActive(false);
    }

    public void OpenCreateRoomPanel()
    {
        CreateRoomPanel.SetActive(true);
    }

    public void OpenJoinRoomPanel()
    {
        JoinRoomPanel.SetActive(true);
    }

    public void BackFromCreateRoom()
    {
        CreateRoomPanel.SetActive(false);
    }

    public void BackFromJoinRoom()
    {
        JoinRoomPanel.SetActive(false);
    }

    public void JoinRoom()
    {

        string text = JoinRoomPanel.transform.Find("RoomNameInputField").GetComponent<InputField>().textComponent.text;
        Debug.Log(text);
        networkController.JoinRoom(text);
        JoinRoomPanel.SetActive(false);
    }
}
