using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject StartMenu = null;
    [SerializeField] private GameObject LobbyMenu = null;
    [SerializeField] private GameObject RoomMenu = null;
    [SerializeField] private LoadScreenMenuController LoadScreenMenuController = null;
    [SerializeField] private GameObject ErrorPanelErrorPrefab = null;
    [SerializeField] private GameObject Canvas = null;
    [SerializeField] private GameObject OptionMenu = null;


    private NetworkController networkController = null;
    private GameController gameController = null;
    private SoundController menuSoundController = null;

    #region UI Actions
    public void EnterLobby()
    {
        menuSoundController.PlayButtonSound();
        StartMenu.SetActive(false);
        LoadScreenMenuController.Show("Joining Lobby...");
        networkController.JoinLobby();
    }
    public void OnBackOptionButtonClicked()
    {
        menuSoundController.PlayButtonSound();
        OptionMenu.SetActive(false);
        StartMenu.SetActive(true);
    }
    public void OnOptionButtonClicked()
    {
        menuSoundController.PlayButtonSound();
        StartMenu.SetActive(false);
        OptionMenu.SetActive(true);
    }
    public void OnLobbyBackButtonClicked()
    {
        menuSoundController.PlayButtonSound();
        LoadScreenMenuController.Show("Disconnecting...");
        networkController.LeaveLobby();
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        menuSoundController.PlayMenuSong();
    }
    private void OnEnable()
    {
        networkController.OnNetworkErrorEvent += CreateMainMenuError;
        networkController.OnJoinedLobbyEvent += OnJoinedLobby;
        networkController.OnJoinedRoomEvent += OnJoinedRoom;
        networkController.OnLeftRoomEvent += OnLeftRoom;
        networkController.OnDisconnectEvent += OnDisconnectEvent;
        gameController.OnLoadGameEvent += OnLoadGame;
        gameController.OnFinishLoadGameEvent += OnFinishLoadGame;
    }

    private void OnDisable()
    {
        networkController.OnNetworkErrorEvent -= CreateMainMenuError;
        networkController.OnJoinedLobbyEvent -= OnJoinedLobby;
        networkController.OnJoinedRoomEvent -= OnJoinedRoom;
        networkController.OnLeftRoomEvent -= OnLeftRoom;
        networkController.OnDisconnectEvent -= OnDisconnectEvent;
        gameController.OnLoadGameEvent -= OnLoadGame;
        gameController.OnFinishLoadGameEvent -= OnFinishLoadGame;
    }
    #endregion
    #region Event Callbacks
    private void OnDisconnectEvent()
    {
        StartMenu.SetActive(true);
        LobbyMenu.SetActive(false);
        LoadScreenMenuController.Hide();
    }
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
    private void OnLoadGame()
    {
        LoadScreenMenuController.Show("Loading Level...");
        LobbyMenu.SetActive(false);
        RoomMenu.SetActive(false);
    }
    private void OnFinishLoadGame()
    {
        LoadScreenMenuController.Hide();
    }
    #endregion
}
