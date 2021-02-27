using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject _startMenu = null;
    [SerializeField] private GameObject _lobbyMenu = null;
    [SerializeField] private GameObject _roomMenu = null;
    [SerializeField] private LoadScreenMenuController _loadScreenMenuController = null;
    [SerializeField] private GameObject _errorPanelErrorPrefab = null;
    [SerializeField] private GameObject _canvas = null;
    [SerializeField] private GameObject _optionMenu = null;

    private NetworkController _networkController = null;
    private GameController _gameController = null;
    private SoundController _menuSoundController = null;

    #region UI Actions
    public void EnterLobby()
    {
        _menuSoundController.PlayButtonSound();
        _startMenu.SetActive(false);
        _loadScreenMenuController.Show("Joining Lobby...");
        _networkController.JoinLobby();
    }
    public void OnBackOptionButtonClicked()
    {
        if(!_gameController.IsGameStart)
        {
            _menuSoundController.PlayButtonSound();
            _optionMenu.SetActive(false);
            _startMenu.SetActive(true);
        }
    }
    public void OnOptionButtonClicked()
    {
        _menuSoundController.PlayButtonSound();
        _startMenu.SetActive(false);
        _optionMenu.SetActive(true);
    }
    public void OnLobbyBackButtonClicked()
    {
        _menuSoundController.PlayButtonSound();
        _loadScreenMenuController.Show("Disconnecting...");
        _networkController.LeaveLobby();
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _menuSoundController.PlayMenuSong();
    }
    private void OnEnable()
    {
        _networkController.OnNetworkErrorEvent += CreateMainMenuError;
        _networkController.OnJoinedLobbyEvent += OnJoinedLobby;
        _networkController.OnJoinedRoomEvent += OnJoinedRoom;
        _networkController.OnLeftRoomEvent += OnLeftRoom;
        _networkController.OnDisconnectEvent += OnDisconnectEvent;
        _gameController.OnLoadGameEvent += OnLoadGame;
        _gameController.OnFinishLoadGameEvent += OnFinishLoadGame;
    }
    private void OnDisable()
    {
        _networkController.OnNetworkErrorEvent -= CreateMainMenuError;
        _networkController.OnJoinedLobbyEvent -= OnJoinedLobby;
        _networkController.OnJoinedRoomEvent -= OnJoinedRoom;
        _networkController.OnLeftRoomEvent -= OnLeftRoom;
        _networkController.OnDisconnectEvent -= OnDisconnectEvent;
        _gameController.OnLoadGameEvent -= OnLoadGame;
        _gameController.OnFinishLoadGameEvent -= OnFinishLoadGame;
    }
    #endregion
    #region Event Callbacks
    private void OnDisconnectEvent()
    {
        _startMenu.SetActive(true);
        _lobbyMenu.SetActive(false);
        _loadScreenMenuController.Hide();
    }
    private void OnJoinedLobby()
    {
        _lobbyMenu.SetActive(true);
        _loadScreenMenuController.Hide();
    }
    private void OnJoinedRoom()
    {
        _roomMenu.SetActive(true);
        _loadScreenMenuController.Hide();
    }
    private void OnLeftRoom()
    {
        _lobbyMenu.SetActive(true);
        _roomMenu.SetActive(false);
    }
    private void CreateMainMenuError(string errorTitle, string errorMessage)
    {
        _loadScreenMenuController.Hide();
        GameObject errorPanelError = Instantiate(_errorPanelErrorPrefab, _canvas.transform);
        ErrorPromptController errorPromptController = errorPanelError.GetComponent<ErrorPromptController>();
        errorPromptController.ErrorTitle = errorTitle;
        errorPromptController.ErrorMessage = errorMessage;
    }
    private void OnLoadGame()
    {
        _loadScreenMenuController.Show("Loading Level...");
        _lobbyMenu.SetActive(false);
        _roomMenu.SetActive(false);
    }
    private void OnFinishLoadGame()
    {
        _loadScreenMenuController.Hide();
    }
    #endregion
}
