using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ErrorPromptController : MonoBehaviour
{
    public string ErrorTitle { get { return _title.text; } set { _title.text = value; } }
    public string ErrorMessage { get { return _message.text; } set { _message.text = value; } }

    [SerializeField] private Text _title = null;
    [SerializeField] private Text _message = null;
    [SerializeField] private GameObject _CancelButton = null;

    private SoundController _menuSoundController = null;
    private EventSystem _eventSystem = null;

    public event Action onOkButtonClicked;

    #region Unity Callbacks
    private void Start()
    {
        _eventSystem = GameObject.FindWithTag("EventSystem").GetComponent<EventSystem>();
        _menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        

        if (_CancelButton != null)
        {
            //_eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_CancelButton);
        }
        else
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(GameObject.Find("OKButton"));
        }

        
    }
    #endregion
    #region UI Actions
    public void Close()
    {
        _menuSoundController.PlayButtonSound();
        onOkButtonClicked?.Invoke();
        Destroy(this.gameObject);
    }
    #endregion
}
