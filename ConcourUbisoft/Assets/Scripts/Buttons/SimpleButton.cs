using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class SimpleButton : MonoBehaviour
{
    [SerializeField] public UnityEvent Actions = new UnityEvent();
    [SerializeField] public UnityEvent BeforeActions = new UnityEvent();
    [SerializeField] public UnityEvent AfterActions = new UnityEvent();

    protected Animator _animator = null;
    protected AudioSource _audioSource;
    protected bool _isHover = false;
    protected bool soundPlayed = false;
    private Outline _outline=null;

    [SerializeField] private int maxDistance = 10;
    private bool _reachable;
    private bool _inPerimeter = false;
    private GameObject _player = null;
    private bool _isPress = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();
        _outline = GetComponentInChildren<Outline>();
        _reachable = false;

        _player = GameObject.FindWithTag("Player");
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
        if (!_reachable && Vector3.Distance(_player.transform.position, gameObject.transform.position) < maxDistance)
        {
            if (_inPerimeter)
            {
                _animator.SetBool("hover", true);
                _isHover = true;
                _outline.enabled = true;
            }
            _reachable = true;
        }

        if (_reachable && Vector3.Distance(_player.transform.position, gameObject.transform.position) >= maxDistance)
        {
            if (_isHover)
            {
                _isHover = false;
                _animator.SetBool("hover", false);
                _outline.enabled = false;
            }
            _reachable = false;
        }
        
        if (_isHover)
        {
            if (GetInput())
            {
                PressButton();
            }
            else
            {
                if(_isPress)
                {
                    AfterActions?.Invoke();
                }
                _isPress = false;
                soundPlayed = false;
            }
        }
        else if(_isPress)
        {
            AfterActions?.Invoke();
            _isPress = false;
        }
    }

    protected virtual void PressButton()
    {
        _animator.SetTrigger("click");
        if (!soundPlayed)
        {
            _audioSource.Play();
            soundPlayed = true;
        }

        if(!_isPress)
        {
            Debug.Log("Before");
            BeforeActions?.Invoke();
        }

        Actions?.Invoke();

        _isPress = true;
    }

    private void OnMouseEnter()
    {
        _inPerimeter = true;
        if (_reachable)
        {
            _animator.SetBool("hover", true);
            _isHover = true;
            _outline.enabled = true;
        }
    }

    private void OnMouseExit()
    {
        _inPerimeter = false;
        if (_isHover)
        {
            _isHover = false;
            _animator.SetBool("hover", false);
            _outline.enabled = false;
        }
    }
}