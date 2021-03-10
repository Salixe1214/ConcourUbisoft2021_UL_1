using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SimpleButton : MonoBehaviour
{
    [SerializeField] public UnityEvent Actions = null;

    protected Animator _animator = null;
    protected AudioSource _audioSource;
    protected bool _isHover = false;
    protected bool soundPlayed = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();
    }

    protected virtual bool GetInput()
    {
        string[] joysticks = Input.GetJoystickNames();
        return Input.GetMouseButtonDown(0) ||
               (joysticks.Contains("Controller (Xbox One For Windows)") && Input.GetButtonDown("ConfirmXBO")) ||
               (joysticks.Contains("Wireless Controller") && Input.GetButtonDown("ConfirmPS"));
    }

    protected virtual void Update()
    {
        if (_isHover)
        {
            if (GetInput())
            {
                PressButon();
            }
            else
            {
                soundPlayed = false;
            }
        }
    }

    protected virtual void PressButon()
    {
        _animator.SetTrigger("click");
        Actions?.Invoke();
        if (!soundPlayed)
        {
            _audioSource.Play();
            soundPlayed = true;
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("Enter Hover");
        _animator.SetBool("hover", true);
        _isHover = true;
    }

    private void OnMouseExit()
    {
        Debug.Log("Exit Hover");
        _isHover = false;
        _animator.SetBool("hover", false);
    }
}