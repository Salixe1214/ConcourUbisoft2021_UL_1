using System.Collections;
using System.Collections.Generic;
using Inputs;
using Other;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _optionMenu = null;
    [SerializeField] private GameObject _inGameMenu = null;
    [SerializeField] private LoadScreenMenuController _loadScreenMenuController = null;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _menuFirstSelected;
    [SerializeField] private GameObject _optionsFirstSelected;
    [SerializeField] private GameObject _optionBackSelected;
    [SerializeField] private GameObject _returnButton;
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private GameObject _creditMenu;
    [SerializeField] private GameObject _creditFirstSelected;

    private NetworkController _networkController = null;
    private GameController _gameController = null;
    private SoundController _soundController = null;
    private InputManager _inputManager;
    private Menus _currentMenu = Menus.InGame;
    private Inputs.Controller _currentController;
    

    public bool IsGameMenuOpen { get; set; } = false;

    #region UI Actions
    public void EnterOptionMenu()
    {
        _soundController.PlayButtonSound();
        _inGameMenu.SetActive(false);
        _optionMenu.SetActive(true);
        _currentMenu = Menus.Options;
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_optionsFirstSelected);
        }
    }
    public void OnBackOptionButtonClicked()
    {
        if(_gameController.IsGameStart)
        {
            _soundController.PlayButtonSound();
            _inGameMenu.SetActive(true);
            _currentMenu = Menus.InGame;
            _optionMenu.SetActive(false);
            if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_optionBackSelected);
            }
        }
    }
    public void ReturnToMenu()
    {
        _soundController.PlayButtonSound();
        _loadScreenMenuController.Show("Returning to menu");
        IsGameMenuOpen = false;
        _gameController.UnLoadGame();
        _inGameMenu.SetActive(false);
    }
    public void ExitGame()
    {
        _soundController.PlayButtonSound();
        Application.Quit();
    }
    public void BackMenu()
    {
        _soundController.PlayButtonSound();
        IsGameMenuOpen = false;
        _inGameMenu.SetActive(false);
        _eventSystem.SetSelectedGameObject(null);
        _gameController.ToggleCursorLock();
    }

    public void OpenCredits()
    {
        _soundController.PlayButtonSound();
        _creditMenu.SetActive(true);
        _inGameMenu.SetActive(false);
        _currentMenu = Menus.Credits;
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_creditFirstSelected);
        }
    }

    public void CloseCredits()
    {
        if(_gameController.IsGameStart)
        {
            _soundController.PlayButtonSound();
            _creditMenu.SetActive(false);
            _optionMenu.SetActive(true);
            _currentMenu = Menus.Options;
            if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_optionsFirstSelected);
            }
        }
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _inputManager = GameObject.FindWithTag("InputManager")?.GetComponent<InputManager>();
        _currentController = InputManager.GetController();
    }
    private void Update()
    {
        if (_gameController.IsGameStart && Input.GetButtonDown("Cancel"))
        {
            if (IsGameMenuOpen)
            {
                _inGameMenu.SetActive(false);
                _optionMenu.SetActive(false);
                ResetButtonTextColor();
            }
            else
            {
                _inGameMenu.SetActive(true);
                _optionMenu.SetActive(false);
                if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(_menuFirstSelected);
                }
            }
            IsGameMenuOpen = !IsGameMenuOpen;
            _gameController.ToggleCursorLock();
        }
    }
    
    private void OnEnable()
    {
        Debug.Log("Subscribed to controller type changed event");
        _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
    }
    private void OnDisable()
    {
        _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
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
            if (_currentMenu == Menus.InGame)
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_menuFirstSelected);
            }
            else if (_currentMenu == Menus.Options)
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(_optionsFirstSelected);
            }
            _currentController = newController;
        }
    }

    private void ResetButtonTextColor()
    {
        _menuFirstSelected.GetComponentInChildren<TextColor>().OnExit();
        _optionBackSelected.GetComponentInChildren<TextColor>().OnExit();
        _returnButton.GetComponentInChildren<TextColor>().OnExit();
        _exitButton.GetComponentInChildren<TextColor>().OnExit();
    }

    #endregion
}
