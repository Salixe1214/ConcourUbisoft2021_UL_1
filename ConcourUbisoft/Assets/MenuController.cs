using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject StartMenu = null;
    [SerializeField] private GameObject LobbyMenu = null;
    [SerializeField] private GameObject RoomMenu = null;
    [SerializeField] private LoadScreenMenuController LoadScreenMenuController = null;
    [SerializeField] private GameObject ErrorPanelErrorPrefab = null;
    [SerializeField] private GameObject Canvas = null;

    private NetworkController networkController = null;

    #region UI Actions
    public void EnterLobby()
    {
        StartMenu.SetActive(false);
        LoadScreenMenuController.Show("Joining Lobby...");
        networkController.JoinLobby();
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
    }
    private void OnEnable()
    {
        networkController.OnNetworkErrorEvent += CreateMainMenuError;
        networkController.OnJoinedLobbyEvent += OnJoinedLobby;
        networkController.OnJoinedRoomEvent += OnJoinedRoom;
        networkController.OnLeftRoomEvent += OnLeftRoom;
    }
    private void OnDisable()
    {
        networkController.OnNetworkErrorEvent -= CreateMainMenuError;
        networkController.OnJoinedLobbyEvent -= OnJoinedLobby;
        networkController.OnJoinedRoomEvent -= OnJoinedRoom;
        networkController.OnLeftRoomEvent -= OnLeftRoom;
    }
    #endregion
    #region Event Callbacks
    private void OnJoinedLobby()
    {
        LobbyMenu.SetActive(true);
        LoadScreenMenuController.Hide();
    }
    private void OnJoinedRoom()
    {
        RoomMenu.SetActive(true);
        LoadScreenMenuController.Hide();
    }
    private void OnLeftRoom()
    {
        LobbyMenu.SetActive(true);
        RoomMenu.SetActive(false);
    }
    private void CreateMainMenuError(string errorTitle, string errorMessage)
    {
        LoadScreenMenuController.Hide();
        GameObject errorPanelError = Instantiate(ErrorPanelErrorPrefab, Canvas.transform);
        ErrorPromptController errorPromptController = errorPanelError.GetComponent<ErrorPromptController>();
        errorPromptController.ErrorTitle = errorTitle;
        errorPromptController.ErrorMessage = errorMessage;
    }
    #endregion
}
