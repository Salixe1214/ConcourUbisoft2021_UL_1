using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using Menu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Serializable]
    public class ColorName
    {
        [SerializeField] private Color _color;
        [SerializeField] private string _name;

        public Color Color { get => _color; set => _color = value; }
        public string Name { get => _name; set => _name = value; }

        public bool IsColor(Color color)
        {
            return Math.Abs(color.r - _color.r) < 0.1f && Math.Abs(color.g - _color.g) < 0.1f && Math.Abs(color.b - _color.b) < 0.1f;
        }
    }

    public enum Role
    {
        SecurityGuard,
        Technician,
        None
    }

    [SerializeField] private string _sceneToStartName = "";
    [SerializeField] private GameObject _audioListener = null;
    [SerializeField] private OptionController _optionController = null;
    [SerializeField] private InGameMenuController _inGameMenuController = null;
    [SerializeField] private GameObject _speaking = null;
    [SerializeField] private ColorName[] _colorNames = null;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private bool _randomSeed = false;
    [SerializeField] private bool forceOrder = false;
    [SerializeField] private bool forceTwoPlayers = false;
    [SerializeField] private EndGameMenuController _endGemMenu= null;

    private SoundController _soundController = null;
    private NetworkController _networkController = null;
    private DialogSystem _dialogSystem = null;
    private InputManager _inputManager;
    private Controller _currentController;
    
    public bool invertedY { get; set; } = false;
    public UnityEvent changeInverted;

    public bool IsGameLoading { get; private set; }
    public bool IsGameStart { get; set; }
    public Role GameRole { get; set; }
    public bool ColorBlindMode { get; set; }
    public OptionController OptionController { get => _optionController; }
    public bool IsGameMenuOpen { get => _inGameMenuController.IsGameMenuOpen; }
    public bool IsEndGameMenuOpen => _endGemMenu.IsEndGameMenuOpen();

    public void CloseInGameMenu() => _inGameMenuController.CloseInGameMenu();
    public int Seed { get; private set; }
    
    public bool ForceOrder { get; private set; }

    public GameObject PauseMenu { get ; private set; }

    public bool ForceTwoPlayers { get; private set; }

    #region Events
    public event Action OnLoadGameEvent;
    public event Action OnFinishLoadGameEvent;
    public event Action OnFinishGameEvent;

    public event Action OnInGameMenuClosed;
    
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _inputManager = GameObject.FindWithTag("InputManager")?.GetComponent<InputManager>();
        ForceOrder = forceOrder;
        PauseMenu = _pauseMenu;
        ForceTwoPlayers = forceTwoPlayers;
        _currentController = InputManager.GetController();
    }
    private void OnEnable()
    {
        _networkController.OnPlayerLeftEvent += OnPlayerLeftEvent;
        _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
        _inGameMenuController.OnInGameMenuClosed += OnInGameMenuClosedInvoked;
    }
    private void OnDisable()
    {
        _networkController.OnPlayerLeftEvent -= OnPlayerLeftEvent;
        _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
        _inGameMenuController.OnInGameMenuClosed -= OnInGameMenuClosedInvoked;
    }
    #endregion
    #region Private Functions
    private IEnumerator LoadAsyncLevel()
    {
        //_soundController.StopMenuSong();
        AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneToStartName, LoadSceneMode.Additive);
        IsGameLoading = true;
        OnLoadGameEvent?.Invoke();
        while (!operation.isDone)
        {
            yield return null;
        }
        IsGameLoading = false;
        IsGameStart = true;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneToStartName));
        Debug.Log("StartDialog");
        _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
        _dialogSystem.StartDialog("Introduction");
        OnFinishLoadGameEvent?.Invoke();
        _speaking.SetActive(true);
        OnControllerTypeChanged();
        //_soundController.PlayAmbientSound();
        if (GameRole == Role.SecurityGuard)
        {
            SetUpSecurityGuard();
        }
        else if(GameRole == Role.Technician)
        {
            SetUpTechnician();
        }
    }
    private IEnumerator UnloadAsyncLevel()
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(_sceneToStartName);
        ResetController();

        while (!operation.isDone)
        {
            yield return null;
        }
        _soundController.StopAmbientSound();
        _soundController.StopAreaMusic();
        _soundController.PlayMenuSong();
       _networkController.LeaveLobby();
    }
    private void ResetController()
    {
        if (GameRole == Role.SecurityGuard)
        {
            GameObject player = GameObject.FindGameObjectWithTag("PlayerGuard");
            Transform playerCamera = Camera.main.transform;
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
        //_soundController.PlayAmbientSound();
        _audioListener.SetActive(true);
        IsGameStart = false;
        GameRole = Role.None;
        Cursor.lockState = CursorLockMode.None;
    }
    private void SetUpSecurityGuard()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameObject playerTech = GameObject.FindGameObjectWithTag("PlayerTech");
        playerTech.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("PlayerGuard");
        _audioListener.SetActive(false);

        //_soundController.PlayAmbientSound();
        GameObject playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerCamera.GetComponent<CameraMovement>().enabled = true;
    }
    private void SetUpTechnician()
    {
        if (_currentController == Controller.Playstation || _currentController == Controller.Xbox)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        GameObject playerTech = GameObject.FindGameObjectWithTag("PlayerTech");
        playerTech.SetActive(true);
        _soundController.MuteAmbient();
        GameObject player = GameObject.FindGameObjectWithTag("PlayerGuard");
        player.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        GameObject playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerCamera.gameObject.SetActive(false);
        playerCamera.GetComponent<AudioListener>().enabled = false;
    }
    private void OnPlayerLeftEvent()
    {
        if (IsGameStart)
        {
            StartCoroutine("UnloadAsyncLevel");
        }
    }

    private void OnInGameMenuClosedInvoked()
    {
        OnInGameMenuClosed?.Invoke();
    }

    private void OnControllerTypeChanged()
    {
        Debug.Log("Controller Type Changed GameController:");
        Inputs.Controller _newControllerType = InputManager.GetController();

        if (_newControllerType == Controller.Playstation || _newControllerType == Controller.Xbox)
        {
            if (GameRole == Role.Technician)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else
        {
            if (GameRole == Role.Technician)
            {
                Cursor.visible = true;
            }
        }

        _currentController = _newControllerType;
    }

    #endregion
    #region Public Functions

    public void InvertY()
    {
        invertedY = !invertedY;
        changeInverted.Invoke();
    }
    public string GetColorName(Color color)
    {
        return _colorNames.Where(x => x.IsColor(color)).FirstOrDefault()?.Name ?? "Undefined";
    }

    public void InitiateStartGame(Role role, bool colorBlind)
    {
        ColorBlindMode = colorBlind;
        if (_randomSeed)
        {
            Seed = new System.Random().Next();
        }
        _networkController.GetOwnNetworkPlayer().StartGameNetwork();
    }

    public void StartGame(Role role, bool colorBlindMode, int seed)
    {
        Debug.Log($"Start Game with role {role}");
        GameRole = role;
        ColorBlindMode = colorBlindMode;
        _soundController.SetSound(GameRole);
        Seed = seed;
        StartCoroutine("LoadAsyncLevel");
    }

    public void UnLoadGame()
    {
        StartCoroutine("UnloadAsyncLevel");
    }
    public void ToggleCursorLock()
    {
        if(GameRole == Role.SecurityGuard)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            GameObject player = GameObject.FindGameObjectWithTag("PlayerGuard");
            Transform playerCamera = Camera.main.transform;
            playerCamera.GetComponent<CameraMovement>().enabled = Cursor.lockState == CursorLockMode.Locked;
        }
    }
    #endregion
}
