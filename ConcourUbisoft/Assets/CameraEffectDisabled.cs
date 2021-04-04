using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraEffectDisabled : MonoBehaviour
{
    [SerializeField] private Volume _volume = null;
    [SerializeField] private Image _image = null;

    public void Enable()
    {
        _image.gameObject.SetActive(true);
        _volume.enabled = true;
    }

    public void Disable()
    {
        _image.gameObject.SetActive(false);
        _volume.enabled = false;
    }
}
