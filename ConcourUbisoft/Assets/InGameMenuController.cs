using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _optionMenu = null;
    [SerializeField] private GameObject _inGameMenu = null;
    [SerializeField] private LoadScreenMenuController _loadScreenMenuController = null;

    private NetworkController _networkController = null;
    private GameController _gameController = null;
    private SoundController _soundController = null;

    public bool IsGameMenuOpen { get; set; } = false;

    #region UI Actions
    public void EnterOptionMenu()
    {
        _soundController.PlayButtonSound();
        _inGameMenu.SetActive(false);
        _optionMenu.SetActive(true);
    }
    public void OnBackOptionButtonClicked()
    {
        if(_gameController.IsGameStart)
        {
            _soundController.PlayButtonSound();
            _inGameMenu.SetActive(true);
            _optionMenu.SetActive(false);
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
        _gameController.ToggleCursorLock();
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }
    private void Update()
    {
        if (_gameController.IsGameStart && Input.GetButtonDown("Cancel"))
        {
            if (IsGameMenuOpen)
            {
                _inGameMenu.SetActive(false);
                _optionMenu.SetActive(false);
            }
            else
            {
                _inGameMenu.SetActive(true);
                _optionMenu.SetActive(false);
            }
            IsGameMenuOpen = !IsGameMenuOpen;
            _gameController.ToggleCursorLock();
        }
    }
    #endregion
}
