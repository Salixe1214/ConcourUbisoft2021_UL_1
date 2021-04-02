using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLight : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.black;
    private List<Color> _color = new List<Color>();
    [SerializeField] private List<GameObject> _indicatorLights;
    private List<Renderer> _renderers = new List<Renderer>();
    
    private int _numYellowLights = 0;

    private void Awake()
    {
        for(int i = 0 ; i < _indicatorLights.Count ; i++)
        {
            _color.Insert(i, defaultColor);
            _renderers.Insert(i, _indicatorLights[i].GetComponent<Renderer>());
        }
    }

    private void Update()
    {
        for(int i = 0 ; i < _indicatorLights.Count ; i++)
        {
            if(i < _numYellowLights)
            {
                _renderers[i].material.color = Color.yellow;
                _renderers[i].material.SetColor("_EmissionColor", Color.yellow * 15);
            }
            else
            {
                _renderers[i].material.color = _color[i];
                _renderers[i].material.SetColor("_EmissionColor", _color[i] * 15);
            }
        }
    }

    public void OnFail()
    {
        _numYellowLights -= _numYellowLights;
        StartCoroutine(Flash(Color.red));
    }

    public void OnSuccess()
    {
        _numYellowLights = 0;
        for(int i = 0 ; i < _indicatorLights.Count ; i++)
        {
            _color[i] = Color.green;
            _renderers[i].material.color = _color[i];
            _renderers[i].material.SetColor("_EmissionColor", _color[i] * 15);
        }
    }

    public void onPressDir()
    {
        if (_numYellowLights < _indicatorLights.Count)
        {
            _numYellowLights += 1;
        }
    }

    IEnumerator Flash(Color pColor)
    {
        for(int i = 0 ; i < _indicatorLights.Count ; i++)
        {
            _color[i] = pColor;
        }
        yield return new WaitForSeconds(4);
        if (_color[0] == Color.red) {
            for (int i = 0; i < _indicatorLights.Count; i++)
            {
                _color[i] = defaultColor;
            }
        }
    }
}
