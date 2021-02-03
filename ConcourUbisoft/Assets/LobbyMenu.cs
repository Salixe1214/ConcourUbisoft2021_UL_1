using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public void AddElement(string name, int playerCount, UnityAction action) {
        GameObject roomListElement = Instantiate(RoomListElementPrefab, ContentPanel.transform);
        roomListElement.transform.Find("RoomName").GetComponent<Text>().text = name;
        roomListElement.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(action);
        roomListElement.transform.Find("PlayerCount").GetComponent<Text>().text = $"{playerCount.ToString()}/2";
    }

    public void ClearElements()
    {
        foreach (Transform child in ContentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateRoom() {
        Debug.Log(TogglePrivate.GetComponent<Toggle>().isOn);
        networkController.CreateRoom(CreateRoomPanel.transform.Find("RoomNameInputField").GetComponent<InputField>().text, TogglePrivate.GetComponent<Toggle>().isOn);
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
        networkController.JoinRoom(JoinRoomPanel.transform.Find("RoomNameInputField").GetComponent<InputField>().text);
    }
}
