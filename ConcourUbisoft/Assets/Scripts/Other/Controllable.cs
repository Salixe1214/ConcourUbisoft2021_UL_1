using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

public class Controllable : MonoBehaviour
{
    public delegate void OnControlStateChangeEventHandler(bool controlled);

    public event OnControlStateChangeEventHandler OnControlStateChange;
    [SerializeField] private bool isControlled = false;

    public bool IsControlled
    {
        get => isControlled;
        set
        {
            if (value != isControlled)
            {
                isControlled = value;
                OnControlStateChange?.Invoke(value);
            }
        }
    }


}