using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class ControlPanelButton : MonoBehaviour
{
    [SerializeField] public UnityEvent Actions = null;
    private Animator _animator = null;

    private bool _isHover = false;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        string[] joysticks = Input.GetJoystickNames();

        if (_isHover)
        {
            if (joysticks.Contains("Controller (Xbox One For Windows)"))
            {
                if (Input.GetButtonDown("ConfirmXBO") && _isHover)
                {
                    _animator.SetTrigger("click");
                    Actions?.Invoke();
                }
            }
            else if (joysticks.Contains("Wireless Controller"))
            {
                if (Input.GetButtonDown("ConfirmPS") && _isHover)
                {
                    _animator.SetTrigger("click");
                    Actions?.Invoke();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _animator.SetTrigger("click");
                Actions?.Invoke();
            }
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
