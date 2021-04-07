using System;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu
{
    public class EndGameMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject EndGameMenu;
        [SerializeField] private GameObject MenuFirstSelected;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private GameObject _confirmationPanel;
        [SerializeField] private LoadScreenMenuController _loadScreenMenuController = null;
        
        private Inputs.Controller _currentController;
        private InputManager _inputManager;
        private bool _isEndMenuOpen;
        private NetworkController _networkController = null;
        private SoundController _soundController = null;
        private GameController _gameController = null;

        private void Awake()
        {
            _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
            _soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
            _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            _inputManager = GameObject.FindWithTag("InputManager")?.GetComponent<InputManager>();
        }

        private void Start()
        {
            _isEndMenuOpen = false;
        }

        private void OnEnable()
        {
            _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
            _networkController.OnDisconnectEvent += OnDisconnectEvent;
        }

        private void OnDisable()
        {
            _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
            _networkController.OnDisconnectEvent -= OnDisconnectEvent;
        }

        private void ShowEndGameMenu()
        {
            EndGameMenu.SetActive(true);
            _isEndMenuOpen = true;
            if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
            {
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(MenuFirstSelected);
            }
        }

        private void OnReturnMenuClicked()
        {
            _soundController.PlayButtonSound();
            _confirmationPanel.SetActive(false);
            _loadScreenMenuController.Show("Returning to menu");
            _isEndMenuOpen = false;
            _gameController.UnLoadGame();
            EndGameMenu.SetActive(false);
        }

        private void OnExitClicked()
        {
            _confirmationPanel.SetActive(false);
            _soundController.PlayButtonSound();
            Application.Quit();
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
                if (_isEndMenuOpen)
                {
                    _eventSystem.SetSelectedGameObject(null);
                    _eventSystem.SetSelectedGameObject(MenuFirstSelected);
                }
                _currentController = newController;
            }
        }
        
        private void OnDisconnectEvent()
        {
            EndGameMenu.SetActive(false);
        }
        
    }
}