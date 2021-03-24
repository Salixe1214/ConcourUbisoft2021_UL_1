using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum Menus
{
 MainMenu,
 Options,
 Lobby,
 Room,
 InGame
}
public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject _startMenu = null;
    [SerializeField] private GameObject _lobbyMenu = null;
    [SerializeField] private GameObject _roomMenu = null;
    [SerializeField] private LoadScreenMenuController _loadScreenMenuController = null;
    [SerializeField] private GameObject _errorPanelErrorPrefab = null;
    [SerializeField] private GameObject _canvas = null;
    [SerializeField] private GameObject _optionMenu = null;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _menuFirstSelected;
    [SerializeField] private GameObject _optionsFirstSelected;
    [SerializeField] private GameObject _lobbyFirstSelected;
    [SerializeField] private GameObject _OptionsBackFirstSelected;
    [SerializeField] private GameObject _lobbyBackFirstSelected;
    [SerializeField] private GameObject _roomFirstSelected;
    [SerializeField] private GameObject _roomBackFirstSelected;
    [SerializeField] private GameObject _lobbyPanelCreateButton;
    [SerializeField] private GameObject _lobbyPanelJoinButton;
    [SerializeField] private GameObject _lobbyPanelRoomNameInputField;
    [SerializeField] private GameObject _lobbyPanelBackButton;

    private NetworkController _networkController = null;
    private GameController _gameController = null;
    private SoundController _menuSoundController = null;
    private InputManager _inputManager;
    private Menus _currentMenu = Menus.MainMenu;
    private Inputs.Controller _currentController;

    #region UI Actions
    public void EnterLobby()
    {
        _menuSoundController.PlayButtonSound();
        _startMenu.SetActive(false);
        _loadScreenMenuController.Show("Joining Lobby...");
        _networkController.JoinLobby();
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_lobbyFirstSelected);
        }
    }
    public void OnBackOptionButtonClicked()
    {
        if(!_gameController.IsGameStart)
        {
            _menuSoundController.PlayButtonSound();
            _optionMenu.SetActive(false);
            _startMenu.SetActive(true);
            _currentMenu = Menus.MainMenu;
            if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_OptionsBackFirstSelected);
            }
        }
    }
    public void OnOptionButtonClicked()
    {
        _menuSoundController.PlayButtonSound();
        _startMenu.SetActive(false);
        _optionMenu.SetActive(true);
        _currentMenu = Menus.Options;
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_optionsFirstSelected);
        }
    }
    public void OnLobbyBackButtonClicked()
    {
        _menuSoundController.PlayButtonSound();
        _loadScreenMenuController.Show("Disconnecting...");
        _networkController.LeaveLobby();
        _currentMenu = Menus.MainMenu;
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_lobbyBackFirstSelected);
        }
    }
    public void ExitGame()
    {
        _menuSoundController.PlayButtonSound();
        Application.Quit();
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _inputManager = GameObject.FindWithTag("InputManager")?.GetComponent<InputManager>();
        //_menuSoundController.PlayMenuSong();
    }

    private void Start()
    {
        _currentController = InputManager.GetController();
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
        _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
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
        _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
    }
    #endregion
    #region Event Callbacks
    private void OnDisconnectEvent()
    {
        _startMenu.SetActive(true);
        _lobbyMenu.SetActive(false);
        _currentMenu = Menus.MainMenu;
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_menuFirstSelected);
        }
        _loadScreenMenuController.Hide();
    }
    private void OnJoinedLobby()
    {
        _lobbyMenu.SetActive(true);
        _currentMenu = Menus.Lobby;
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_lobbyFirstSelected);
        }
        _loadScreenMenuController.Hide();
    }
    private void OnJoinedRoom()
    {
        _roomMenu.SetActive(true);
        _currentMenu = Menus.Room;
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_roomFirstSelected);
        }
        _loadScreenMenuController.Hide();
    }
    private void OnLeftRoom()
    {
        _lobbyMenu.SetActive(true);
        _currentMenu = Menus.Lobby;
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_roomBackFirstSelected);
        }
        _roomMenu.SetActive(false);
    }
    private void CreateMainMenuError(string errorTitle, string errorMessage)
    {
        _loadScreenMenuController.Hide();
        GameObject errorPanelError = Instantiate(_errorPanelErrorPrefab, _canvas.transform);
        ErrorPromptController errorPromptController = errorPanelError.GetComponent<ErrorPromptController>();
        errorPromptController.ErrorTitle = errorTitle;
        errorPromptController.ErrorMessage = errorMessage;
        _lobbyMenu.SetActive(true);
        _currentMenu = Menus.Lobby;
        _lobbyPanelCreateButton.SetActive(true);
        _lobbyPanelJoinButton.SetActive(true);
        _lobbyPanelRoomNameInputField.SetActive(true);
        _lobbyPanelBackButton.SetActive(true);
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

    private void OnControllerTypeChanged()
    {
        Inputs.Controller newController = InputManager.GetController();
        if (newController == Controller.Other)
        {
            _eventSystem.SetSelectedGameObject(null);
            _currentController = newController;
        }
        else
        {
            if (_currentController == Controller.Playstation || _currentController == Controller.Xbox )
            {
                _currentController = newController;
            }
            else
            {
                if (_currentMenu == Menus.MainMenu)
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(_menuFirstSelected);
                }
                else if (_currentMenu == Menus.Options)
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(_optionsFirstSelected);
                }
                else if (_currentMenu == Menus.Lobby)
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(_lobbyFirstSelected);
                }
                else if (_currentMenu == Menus.Room)
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(_roomFirstSelected);
                }
                _currentController = newController;
            }
             
        }

        
    }

    #endregion
}
