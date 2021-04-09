using System;
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
    [SerializeField] private GameObject _confirmationPanel;
    [SerializeField] private Text _confirmationText;
    [SerializeField] private GameObject _confirmExitButton;
    [SerializeField] private GameObject _confirmReturnButton;

    private NetworkController _networkController = null;
    private GameController _gameController = null;
    private SoundController _soundController = null;
    private InputManager _inputManager;
    private Menus _currentMenu = Menus.InGame;
    private Inputs.Controller _currentController;
    

    public bool IsGameMenuOpen { get; set; } = false;

    public event Action OnInGameMenuClosed;

    #region UI Actions
    public void EnterOptionMenu()
    {
        _soundController.PlayButtonSound();
        _inGameMenu.SetActive(false);
        _optionMenu.SetActive(true);
        _confirmationPanel.SetActive(false);
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

    public void TriggerReturnToMenu()
    {
        _soundController.PlayButtonSound();
        _confirmationText.text = "Return to menu";
        
        _confirmReturnButton.SetActive(true);
        _confirmExitButton.SetActive(false);
        _confirmationPanel.SetActive(true);
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_confirmReturnButton);
        }
    }

    public void TriggerExit()
    {
        _soundController.PlayButtonSound();
        _confirmationText.text = "Exit the game";
        
        _confirmReturnButton.SetActive(false);
        _confirmExitButton.SetActive(true);
        _confirmationPanel.SetActive(true);
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_confirmExitButton);
        }
    }

    public void CancelTrigger()
    {
        _soundController.PlayButtonSound();
        _confirmationPanel.SetActive(false);
        if (_currentController == Controller.Xbox || _currentController == Controller.Playstation)
        {
            _eventSystem.SetSelectedGameObject(_menuFirstSelected);
        }
    }
    public void ReturnToMenu()
    {
        _soundController.PlayButtonSound();
        _confirmationPanel.SetActive(false);
        _loadScreenMenuController.Show("Returning to menu");
        IsGameMenuOpen = false;
        _gameController.UnLoadGame();
        _inGameMenu.SetActive(false);
    }
    public void ExitGame()
    {
        _confirmationPanel.SetActive(false);
        _soundController.PlayButtonSound();
        Application.Quit();
    }
    public void BackMenu()
    {
        _soundController.PlayButtonSound();
        IsGameMenuOpen = false;
        _inGameMenu.SetActive(false);
        _eventSystem.SetSelectedGameObject(null);
        if (_currentController == Controller.Other)
        {
            _gameController.ToggleCursorLock();
        }
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
        if (_gameController.IsGameStart&&!_gameController.IsEndGameMenuOpen &&(
            (_currentController == Controller.Other && Input.GetButtonUp("Cancel"))||
             (_currentController == Controller.Playstation &&Input.GetButtonUp("InGameMenuPS"))||
             (_currentController == Controller.Xbox&& Input.GetButtonUp("InGameMenuXBO"))))
        {
            if (IsGameMenuOpen)
            {
                ResetButtonTextColor();
                _inGameMenu.SetActive(false);
                _optionMenu.SetActive(false);
                IsGameMenuOpen = false;
                OnInGameMenuClosed?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
                if (_currentController == Controller.Other)
                {
                    _gameController.ToggleCursorLock();
                }
            }
            else
            {
                _inGameMenu.SetActive(true);
                IsGameMenuOpen = true;
                //_optionMenu.SetActive(false);
                if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
                {
                     Cursor.visible = false;
                     _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(_menuFirstSelected);
                }
                else
                {
                    _gameController.ToggleCursorLock();
                }
            }
        }
    }
    
    private void OnEnable()
    {
        Debug.Log("Subscribed to controller type changed event");
        _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
        _networkController.OnDisconnectEvent += OnDisconnectEvent;
        //_eventSystem.SetSelectedGameObject(_menuFirstSelected);
    }

    private void OnDisable()
    {
        _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
        _networkController.OnDisconnectEvent -= OnDisconnectEvent;
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

    public void CloseInGameMenu()
    {
        if (IsGameMenuOpen)
        {
            ResetButtonTextColor();
            _inGameMenu.SetActive(false);
            _optionMenu.SetActive(false);
            IsGameMenuOpen = false;
            _eventSystem.SetSelectedGameObject(null);
        }
    }

    #endregion

    #region Private
    private void OnDisconnectEvent()
    {
        IsGameMenuOpen = false;
        _inGameMenu.SetActive(false);
        _confirmReturnButton.SetActive(false);
        _confirmExitButton.SetActive(false);
        _confirmationPanel.SetActive(false);
    }
    #endregion
}
