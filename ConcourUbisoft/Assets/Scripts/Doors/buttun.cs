using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttun : MonoBehaviour
{
    private Material _buttonMaterial;
    // Start is called before the first frame update
    void Awake()
    {
        _buttonMaterial = gameObject.GetComponent<Renderer>().material;
        _buttonMaterial.SetColor("_Color", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        _buttonMaterial.SetColor("_Color", Color.cyan);
        StartCoroutine(ColorFalsh());
        GetComponentInParent<doorsScript>().ButtonPressed(this.name);
    }

    IEnumerator ColorFalsh()
    {
        yield return new WaitForSeconds(0.33f);
        _buttonMaterial.SetColor("_Color", Color.blue);
    }
}
