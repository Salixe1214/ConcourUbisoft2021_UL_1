using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private string SceneToStartName = "";

    public bool IsGameLoading { get; private set; }
    public bool IsGameStart { get; private set; }

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
    }
    #endregion
    #region Public Functions
    public void StartGame()
    {
        StartCoroutine("LoadAsyncLevel");
    }
    #endregion
}
