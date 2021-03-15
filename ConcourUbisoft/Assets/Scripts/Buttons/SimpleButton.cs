using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class SimpleButton : MonoBehaviour
{
    [SerializeField] public UnityEvent Actions = null;

    protected Animator _animator = null;
    protected AudioSource _audioSource;
    protected bool _isHover = false;
    protected bool soundPlayed = false;
    private Outline _outline=null;

    private int _maxDistance = 10;
    private bool _reachable;
    private bool _inPerimeter = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();
        _outline = GetComponentInChildren<Outline>();
        _reachable = false;
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
        if (!_reachable && Vector3.Distance(GameObject.FindWithTag("Player").transform.position, gameObject.transform.position) < _maxDistance)
        {
            if (_inPerimeter)
            {
                Debug.Log("Enter Hover");
                _animator.SetBool("hover", true);
                _isHover = true;
                _outline.enabled = true;
            }
            _reachable = true;
        }

        if (_reachable && Vector3.Distance(GameObject.FindWithTag("Player").transform.position, gameObject.transform.position) >= _maxDistance)
        {
            if (_isHover)
            {
                Debug.Log("Exit Hover");
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
                soundPlayed = false;
            }
        }
    }

    protected virtual void PressButton()
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
        _inPerimeter = true;
        if (_reachable)
        {
            Debug.Log("Enter Hover");
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
            Debug.Log("Exit Hover");
            _isHover = false;
            _animator.SetBool("hover", false);
            _outline.enabled = false;
        }
    }
}