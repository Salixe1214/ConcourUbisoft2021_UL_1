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

    public bool IsGameLoading { get; private set; }
    public bool IsGameStart { get; private set; }
    public Role GameRole { get; set; }

    #region Events
    public delegate void OnLoadGameHandler();
    public event OnLoadGameHandler OnLoadGameEvent;

    public delegate void OnFinishLoadGameHander();
    public event OnFinishLoadGameHander OnFinishLoadGameEvent;
    #endregion
    #region Private Functions
    private IEnumerator LoadAsyncLevel()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToStartName, LoadSceneMode.Additive);
        IsGameLoading = true;
        OnLoadGameEvent?.Invoke();
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        IsGameLoading = false;
        OnFinishLoadGameEvent?.Invoke();
        IsGameStart = true;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneToStartName));

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
        player.GetComponent<CharacterControl>().enabled = true;
        player.transform.Find("Main Camera").gameObject.SetActive(true);
    }
    private void SetUpTechnician()
    {
        GameObject playerTech = GameObject.FindGameObjectWithTag("PlayerTech");
        playerTech.SetActive(true);
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
