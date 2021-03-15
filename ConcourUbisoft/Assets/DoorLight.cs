using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLight : MonoBehaviour
{
    [SerializeField] private Light _light = null;
    [SerializeField] private Renderer _renderer = null;
    [SerializeField] private Color _color = Color.black;

    private void Update()
    {
        _light.color = _color;
        _renderer.material.color = _color;
        _renderer.material.SetColor("_EmissionColor", _color);
    }
}
