using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;
using UnityEngine.EventSystems;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject _roomListElementPrefab = null;
    [SerializeField] private GameObject _contentPanel = null;
    //[SerializeField] private GameObject _createRoomPanel = null;
    //[SerializeField] private GameObject _togglePrivate = null;
    //[SerializeField] private GameObject _joinRoomPanel = null;
    [SerializeField] private GameObject _roomNameCreateInputField = null;
    [SerializeField] private GameObject _roomNameJoinInputField = null;
    [SerializeField] private LoadScreenMenuController _loadScreenMenuController = null;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _createRoomPanelFirstSelected;
    [SerializeField] private GameObject _createRoomPanelBackFirstSelected;
    //[SerializeField] private GameObject _joinRoomPanelFirstSelected;
    //[SerializeField] private GameObject _joinRoomPanelBackFirstSelected;

    [SerializeField] private GameObject _createButton;
    [SerializeField] private GameObject _roomNameInput;
    [SerializeField] private GameObject _backButton;
    [SerializeField] private GameObject _lobbyListHeader;
 
    private NetworkController _networkController = null;
    private SoundController _menuSoundController = null;

    #region UI Actions
    public void OpenCreateRoomPanel()
    {
        _menuSoundController.PlayButtonSound();
        //_createRoomPanel.SetActive(true);
        _createButton.SetActive(false);
        _backButton.SetActive(false);
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_createRoomPanelFirstSelected);
    }
    public void CreateRoom()
    {
        _menuSoundController.PlayButtonSound();
        _networkController.CreateRoom(_roomNameCreateInputField.GetComponent<InputField>().text, false);
        _loadScreenMenuController.Show("Creating Room...");
        _createButton.SetActive(false);
        _roomNameInput.SetActive(false);
        _backButton.SetActive(false);
        _lobbyListHeader.SetActive(false);
        //_createRoomPanel.SetActive(false);
    }
    public void BackFromCreateRoom()
    {
        _menuSoundController.PlayButtonSound();
        //_createRoomPanel.SetActive(false);
        _createButton.SetActive(true);
        _backButton.SetActive(true);
        _lobbyListHeader.SetActive(true);
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_createRoomPanelBackFirstSelected);
    }
    public void OpenJoinRoomPanel()
    {
        _menuSoundController.PlayButtonSound();
        //_joinRoomPanel.SetActive(true);
        _createButton.SetActive(false);
        _backButton.SetActive(false);
        _lobbyListHeader.SetActive(false);
        _eventSystem.SetSelectedGameObject(null);
       // _eventSystem.SetSelectedGameObject(_joinRoomPanelFirstSelected);
    }
    public void JoinRoom()
    {
        _menuSoundController.PlayButtonSound();
        string text = _roomNameJoinInputField.GetComponent<InputField>().textComponent.text;
        _loadScreenMenuController.Show("Joining Room...");
        _createButton.SetActive(false);
        _roomNameInput.SetActive(false);
        _backButton.SetActive(false);
        _lobbyListHeader.SetActive(false);
        _networkController.JoinRoom(text);
        //_joinRoomPanel.SetActive(false);
    }
    public void BackFromJoinRoom()
    {
        _menuSoundController.PlayButtonSound();
        //_joinRoomPanel.SetActive(false);
        _createButton.SetActive(true);
        _roomNameInput.SetActive(true);
        _backButton.SetActive(true);
        _lobbyListHeader.SetActive(true);
        _eventSystem.SetSelectedGameObject(null);
        //_eventSystem.SetSelectedGameObject(_joinRoomPanelBackFirstSelected);
    }
    public void OnTogglePrivate()
    {
        _menuSoundController.PlayButtonSound();
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }
    private void OnEnable()
    {
        _networkController.OnRoomListUpdateEvent += NetworkController_OnLobbyRoomListUpdate;
    }
    private void OnDisable()
    {
        _networkController.OnRoomListUpdateEvent -= NetworkController_OnLobbyRoomListUpdate;
    }
    #endregion
    #region Event Callbacks
    private void NetworkController_OnLobbyRoomListUpdate(IEnumerable<NetworkController.RoomInformation> roomInformations)
    {
        ClearElementsOfLobbyList();

        roomInformations.Where(x => x.PlayerCount != 0 && x.PlayerCount != 2).All((roomInfo) => {
            AddElementToLobbyList(roomInfo.RoomName, roomInfo.PlayerCount, () => { OnJoinButtonClick(roomInfo.RoomName); }); return true;
        });
    }
    #endregion
    #region Private Functions
    private void AddElementToLobbyList(string name, int playerCount, UnityAction action)
    {
        GameObject roomListElement = Instantiate(_roomListElementPrefab, _contentPanel.transform);
        roomListElement.transform.Find("RoomName").GetComponent<Text>().text = name;
        roomListElement.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(action);
        roomListElement.transform.Find("PlayerCount").GetComponent<Text>().text = $"{playerCount.ToString()}/2";
    }
    private void ClearElementsOfLobbyList()
    {
        foreach (Transform child in _contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void OnJoinButtonClick(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        _menuSoundController.PlayButtonSound();
    }
    #endregion
}
