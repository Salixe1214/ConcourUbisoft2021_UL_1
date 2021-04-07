using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Voice.Unity;
using UnityEngine;

public class DoorLight : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.black;
    private List<Color> _color = new List<Color>();
    [SerializeField] private List<GameObject> _indicatorLights;
    private List<Renderer> _renderers = new List<Renderer>();
    [SerializeField] private GameObject topLight;
    private Renderer topLightRenderer;
    
    
    private int _numYellowLights = 0;

    private void Awake()
    {
        for(int i = 0 ; i < _indicatorLights.Count ; i++)
        {
            _color.Insert(i, defaultColor);
            _renderers.Insert(i, _indicatorLights[i].GetComponent<Renderer>());
        }

        topLightRenderer = topLight.GetComponent<Renderer>();
        topLightRenderer.material.color = defaultColor;
        topLightRenderer.material.SetColor("_EmissionColor", defaultColor * 15);
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
        defaultColor = Color.green;
        for(int i = 0 ; i < _indicatorLights.Count ; i++)
        {
            _color[i] = Color.green;
            _renderers[i].material.color = _color[i];
            _renderers[i].material.SetColor("_EmissionColor", _color[i] * 15);
        }
        topLightRenderer.material.color = Color.green;
        topLightRenderer.material.SetColor("_EmissionColor", Color.green * 15);
    }

    public void onPressDir()
    {
        if (_numYellowLights < _indicatorLights.Count)
        {
            _numYellowLights += 1;
        }
    }

    public void OnCLose()
    {
        for(int i = 0 ; i < _indicatorLights.Count ; i++)
        {
            _color[i] = Color.black;
        }
        topLightRenderer.material.color = Color.black;
        topLightRenderer.material.SetColor("_EmissionColor", Color.black * 15);
    }

    IEnumerator Flash(Color pColor)
    {
        for(int i = 0 ; i < _indicatorLights.Count ; i++)
        {
            _color[i] = pColor;
        }
        topLightRenderer.material.color = pColor;
        topLightRenderer.material.SetColor("_EmissionColor", pColor * 15);
        yield return new WaitForSeconds(4);
        if (_color[0] == Color.red) {
            for (int i = 0; i < _indicatorLights.Count; i++)
            {
                _color[i] = defaultColor;
            }
        }
        
        topLightRenderer.material.color = defaultColor;
        topLightRenderer.material.SetColor("_EmissionColor", defaultColor * 15);
    }
}
