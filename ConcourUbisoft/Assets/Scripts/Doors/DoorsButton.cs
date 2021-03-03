using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorsButton : MonoBehaviour
{
    public Action<DoorsScript.ButtonType> ButtonPressed;
    public bool clickable;

    [SerializeField] private DoorsScript.ButtonType bType;
    
    private Material _buttonMaterial;
    private Color _originalColor, _clickColor, _hoverColor;

    private bool _isHover = false;
    private bool _isActivated = false;

    void Awake()
    {
        _buttonMaterial = gameObject.GetComponent<Renderer>().material;
        
        _originalColor = Color.grey; // Default color of the button
        _clickColor = Color.blue; // Color when clicked
        _hoverColor = Color.cyan; // Hovered color
        
        _buttonMaterial.SetColor("_Color", _originalColor);

        clickable = true;
    }

    private void Update()
    {
        string[] joysticks = Input.GetJoystickNames();

        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
            if (Input.GetButtonDown("ConfirmXBO") && _isHover)
            {
                OnMouseDown();
            }
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            if (Input.GetButtonDown("ConfirmPS") && _isHover)
            {
                OnMouseDown();
            }
        }
    }
    

    private void OnMouseDown()
    {
        if (clickable)
        {
            if (bType != DoorsScript.ButtonType.Confirm)
            {
                _buttonMaterial.SetColor("_Color", _clickColor);
                StartCoroutine(ColorFlash());
            }
        
            if(ButtonPressed != null)
                ButtonPressed.Invoke(bType);
        }
    }

    private void OnMouseEnter()
    {
        if(!_isActivated)
            _buttonMaterial.SetColor("_Color", _hoverColor);
        _isHover = true;
    }

    private void OnMouseExit()
    {
        if(!_isActivated)
            _buttonMaterial.SetColor("_Color", _originalColor);
        
        _isHover = false;
    }

    IEnumerator ColorFlash()
    {
        _isActivated = true;
        
        yield return new WaitForSeconds(0.5f);
        
        if(_isHover)
            _buttonMaterial.SetColor("_Color", _hoverColor);
        else
            _buttonMaterial.SetColor("_Color", _originalColor);

        _isActivated = false;
    }
}
