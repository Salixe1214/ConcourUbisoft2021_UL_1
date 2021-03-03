using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPromptController : MonoBehaviour
{
    public string ErrorTitle { get { return _title.text; } set { _title.text = value; } }
    public string ErrorMessage { get { return _message.text; } set { _message.text = value; } }

    [SerializeField] private Text _title = null;
    [SerializeField] private Text _message = null;

    private SoundController _menuSoundController = null;

    #region Unity Callbacks
    private void Start()
    {
        _menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }
    #endregion
    #region UI Actions
    public void Close()
    {
        _menuSoundController.PlayButtonSound();
        Destroy(this.gameObject);
    }
    #endregion
}
