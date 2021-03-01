using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
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

    private SoundController _soundController = null;
    private NetworkController _networkController = null;
    
    public bool IsGameLoading { get; private set; }
    public bool IsGameStart { get; set; }
    public Role GameRole { get; set; }
    public OptionController OptionController { get => _optionController; }
    public bool IsGameMenuOpen { get => _inGameMenuController.IsGameMenuOpen; }

    #region Events
    public event Action OnLoadGameEvent;
    public event Action OnFinishLoadGameEvent;
    public event Action OnFinishGameEvent;
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        UnityEngine.Random.InitState(0);
        _soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
    }
    private void OnEnable()
    {
        _networkController.OnPlayerLeftEvent += OnPlayerLeftEvent;
    }
    private void OnDisable()
    {
        _networkController.OnPlayerLeftEvent -= OnPlayerLeftEvent;
    }
    #endregion
    #region Private Functions
    private IEnumerator LoadAsyncLevel()
    {
        _soundController.StopMenuSong();
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
        OnFinishLoadGameEvent?.Invoke();
        _soundController.PlayAmbientSound();
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
        _networkController.LeaveLobby();
    }
    private void ResetController()
    {
        if (GameRole == Role.SecurityGuard)
        {
            GameObject player = GameObject.FindGameObjectWithTag("PlayerGuard");
            Transform playerCamera = player.transform.Find("Main Camera");
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
        _soundController.PlayAmbientSound();
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
        Transform playerCamera = player.transform.Find("Main Camera");
        playerCamera.GetComponent<CameraMovement>().enabled = true;
    }
    private void SetUpTechnician()
    {
        Cursor.lockState = CursorLockMode.None;
        GameObject playerTech = GameObject.FindGameObjectWithTag("PlayerTech");
        playerTech.SetActive(true);

        GameObject player = GameObject.FindGameObjectWithTag("PlayerGuard");
        player.GetComponent<CharacterControl>().enabled = false;
        Transform playerCamera = player.transform.Find("Main Camera");
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
    #endregion
    #region Public Functions
    public void StartGame(Role role)
    {
        Debug.Log($"Start Game with role {role}");
        GameRole = role;
        _soundController.SetSound(GameRole);
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
            Transform playerCamera = player.transform.Find("Main Camera");
            playerCamera.GetComponent<CameraMovement>().enabled = Cursor.lockState == CursorLockMode.Locked;
        }
    }
    #endregion
}
