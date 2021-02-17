using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPromptController : MonoBehaviour
{
    public string ErrorTitle { get { return Title.text; } set { Title.text = value; } }
    public string ErrorMessage { get { return Message.text; } set { Message.text = value; } }

    [SerializeField] private Text Title = null;
    [SerializeField] private Text Message = null;

    private SoundController menuSoundController = null;

    #region Unity Callbacks
    private void Start()
    {
        menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }
    #endregion
    #region UI Actions
    public void Close()
    {
        menuSoundController.PlayButtonSound();
        Destroy(this.gameObject);
    }
    #endregion
}
