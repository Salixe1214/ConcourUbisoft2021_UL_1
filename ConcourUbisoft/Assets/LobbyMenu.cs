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
    [SerializeField] private GameObject CreateRoomNameInputField = null;
    [SerializeField] private GameObject TogglePrivate = null;

    private NetworkController networkController = null;

    private void Awake()
    {
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
    }

    public void AddElement(string name, UnityAction action) {
        GameObject roomListElement = Instantiate(RoomListElementPrefab, ContentPanel.transform);
        roomListElement.transform.Find("RoomName").GetComponent<Text>().text = name;
        roomListElement.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(action);
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
        networkController.CreateRoom(CreateRoomNameInputField.GetComponent<InputField>().text, TogglePrivate.GetComponent<Toggle>().isOn);
    }

    public void OnCreateRoomFromLobbyClicked()
    {
        //LobbyPanel.SetActive(false);
        CreateRoomPanel.SetActive(true);
    }

    public void OnBackFromCreateRoom()
    {
        //LobbyPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);
    }
}
