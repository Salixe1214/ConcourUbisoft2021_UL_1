using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum Role
    {
        A,
        B
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

        if (GameRole == Role.A)
        {
            SetUpA();
        }
        else if(GameRole == Role.B)
        {
            SetUpB();
        }
    }
    private void SetUpA()
    {
        
    }
    private void SetUpB()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<CharacterControl>().enabled = false;
        player.transform.Find("Main Camera").gameObject.SetActive(false);
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
