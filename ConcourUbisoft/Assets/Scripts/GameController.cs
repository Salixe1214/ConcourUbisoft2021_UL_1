using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum Role
    {
        SecurityGuard,
        Technician
    }

    [SerializeField] private string SceneToStartName = "";
    [SerializeField] private GameObject AudioListener = null;
    [SerializeField] private OptionController optionController = null;

    private SoundController soundController = null;

    public bool IsGameLoading { get; private set; }
    public bool IsGameStart { get; private set; }
    public Role GameRole { get; set; }
    public OptionController OptionController { get => optionController; }

    #region Events
    public delegate void OnLoadGameHandler();
    public event OnLoadGameHandler OnLoadGameEvent;

    public delegate void OnFinishLoadGameHander();
    public event OnFinishLoadGameHander OnFinishLoadGameEvent;

    public event System.Action OnFinishGameEvent;
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
       Random.InitState(0);
       soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }
    #endregion
    #region Private Functions
    private IEnumerator LoadAsyncLevel()
    {
        soundController.StopMenuSong();
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToStartName, LoadSceneMode.Additive);
        IsGameLoading = true;
        OnLoadGameEvent?.Invoke();
        while (!operation.isDone)
        {
            yield return null;
        }
        IsGameLoading = false;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneToStartName));
        OnFinishLoadGameEvent?.Invoke();
        IsGameStart = true;
        if (GameRole == Role.SecurityGuard)
        {
            SetUpSecurityGuard();
        }
        else if(GameRole == Role.Technician)
        {
            SetUpTechnician();
        }
    }

    private void SetUpSecurityGuard()
    {
        GameObject playerTech = GameObject.FindGameObjectWithTag("PlayerTech");
        playerTech.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("PlayerGuard");
        AudioListener.SetActive(false);
        soundController.PlayAmbientSound();
        Transform playerCamera = player.transform.Find("Main Camera");
        playerCamera.GetComponent<CameraMovement>().enabled = true;
    }
    private void SetUpTechnician()
    {
        GameObject playerTech = GameObject.FindGameObjectWithTag("PlayerTech");
        playerTech.SetActive(true);
        AudioListener.transform.parent = playerTech.transform;
        AudioListener.transform.localPosition = Vector3.zero;

        GameObject player = GameObject.FindGameObjectWithTag("PlayerGuard");
        player.GetComponent<CharacterControl>().enabled = false;
        Transform playerCamera = player.transform.Find("Main Camera");
        playerCamera.gameObject.SetActive(false);
        playerCamera.GetComponent<AudioListener>().enabled = false;
    }
    #endregion
    #region Public Functions
    public void StartGame(Role role)
    {
        Debug.Log($"Start Game with role {role}");
        GameRole = role;
        StartCoroutine("LoadAsyncLevel");
    }
    #endregion
}
