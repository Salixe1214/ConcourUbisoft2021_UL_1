using System;
using System.Collections;
using System.Collections.Generic;
using Arm;
using UnityEngine;
using System.Linq;

public class RoboticArmController : MonoBehaviour
{
    enum Usage
    {
        Up, Down, Left, Right, Toggle, Catch
    }
    
    [SerializeField] private ArmController Arm;
    [SerializeField] private MagnetController Magnet;
    [SerializeField] private Usage bType;
    
    static bool _isToggle = false;
    
    private Material _buttonMaterial;
    private Color _originalColor, _clickColor, _hoverColor;

    private bool _isHover = false;
    static bool _isActivated = false;
    
    void Awake()
    {
        _buttonMaterial = gameObject.GetComponent<Renderer>().material;
        
        _originalColor = Color.grey; // Default color of the button
        _clickColor = Color.blue; // Color when clicked
        _hoverColor = Color.cyan; // Hovered color
        
        _buttonMaterial.SetColor("_Color", _originalColor);

    }

    private void OnMouseDown()
    {
        _buttonMaterial.SetColor("_Color", _clickColor);

        if (!_isToggle)
        {
            switch (bType)
            {
                case Usage.Right:
                    Arm.OnVMove(1);
                    break;
                case Usage.Left:
                    Arm.OnVMove(-1);
                    break;
                case Usage.Down:
                    Arm.OnHMove(1);
                    break;
                case Usage.Up:
                    Arm.OnHMove(-1);
                    break;
                case Usage.Toggle:
                    _isToggle = true;
                    break;
                case Usage.Catch:
                    Magnet.ToggleMagnet();
                    break; 
            }
            
            _isActivated = true;
        }
        else
        {
            if (_isActivated)
            {
                switch (bType)
                {
                    case Usage.Toggle:
                        _isToggle = false;
                        break;
                    default:
                        Arm.OnHMove(0);
                        Magnet.ToggleMagnet();
                        Arm.OnVMove(0);
                        if(_isHover)
                            _buttonMaterial.SetColor("_Color", _hoverColor);
                        else
                            _buttonMaterial.SetColor("_Color", _originalColor);
                        break;
                }

                _isActivated = false;
            }
            else
            {
                switch (bType)
                {
                    case Usage.Right:
                        Arm.OnVMove(1);
                        break;
                    case Usage.Left:
                        Arm.OnVMove(-1);
                        break;
                    case Usage.Down:
                        Arm.OnHMove(1);
                        break;
                    case Usage.Up:
                        Arm.OnHMove(-1);
                        break;
                    case Usage.Toggle:
                        _isToggle = false;
                        break;
                    case Usage.Catch:
                        Magnet.ToggleMagnet();
                        break; 
                }

                _isActivated = true;
            }
        }
    }

    private void OnMouseUp()
    {
        if(_isHover)
            _buttonMaterial.SetColor("_Color", _hoverColor);
        else
            _buttonMaterial.SetColor("_Color", _originalColor);

        if (!_isToggle)
        {
            switch (bType)
            {
                case Usage.Up:
                    Arm.OnHMove(0);
                    break;
                case Usage.Down:
                    Arm.OnHMove(0);
                    break;
                case Usage.Left:
                    Arm.OnVMove(0);
                    break;
                case Usage.Right:
                    Arm.OnVMove(0);
                    break;
                case Usage.Toggle:
                    break;
                case Usage.Catch:
                    Magnet.ToggleMagnet();
                    break;
            }
            _isActivated = false;
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

        if (!_isToggle)
        {
            switch (bType)
            {
                case Usage.Up:
                    Arm.OnHMove(0);
                    break;
                case Usage.Down:
                    Arm.OnHMove(0);
                    break;
                case Usage.Left:
                    Arm.OnVMove(0);
                    break;
                case Usage.Right:
                    Arm.OnVMove(0);
                    break;
                case Usage.Toggle:
                    break;
                case Usage.Catch:
                    Magnet.ToggleMagnet();
                    break;
            }
            
            _isActivated = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        string[] joysticks = Input.GetJoystickNames();

        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
            if (Input.GetButtonDown("ConfirmXBO") && _isHover)
            {
                OnMouseDown();
            }
            if (Input.GetButtonUp("ConfirmXBO") && _isHover)
            {
                OnMouseUp();
            }
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            if (Input.GetButtonDown("ConfirmPS") && _isHover)
            {
                OnMouseDown();
            }
            if (Input.GetButtonUp("ConfirmPS") && _isHover)
            {
                OnMouseUp();
            }
        }
    }
}
