using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class NetworkSync : MonoBehaviour
{
    [SerializeField] private Component _componentToSync = null;
    [SerializeField] private GameController.Role _owner = GameController.Role.SecurityGuard;
    [SerializeField] private bool _smooth = true;

    public GameController.Role Owner
    {
        get => _owner;
        set => _owner = value;
    }

    private float _lag;
    private double _lastTime = 0;
    private double _currentTime = 0;
    private byte[] _oldData = null;
    private byte[] _newData = null;

    private void Awake()
    {
        if (!(_componentToSync is Serializable))
        {
            Debug.LogError($"The component: {_componentToSync.name} must implement the interface ISerializable.");
        }
    }

    public byte[] Serialize()
    {
        return (_componentToSync as Serializable).Serialize();
    }

    public void Deserialize(byte[] data, float lag, double currentTime)
    {
        _lag = lag;
        _oldData = _newData;
        _newData = data;
        _lastTime = _currentTime;
        _currentTime = currentTime;
        (_componentToSync as Serializable).Deserialize(data);
    }

    public void Smooth()
    {
        if (_oldData != null && _smooth && _lastTime != 0)
        {
            (_componentToSync as Serializable).Smooth(_oldData, _newData, _lag, _lastTime, _currentTime);
        }
    }
}