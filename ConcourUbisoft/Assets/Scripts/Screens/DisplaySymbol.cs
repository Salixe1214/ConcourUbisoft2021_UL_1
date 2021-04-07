using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySymbol : MonoBehaviour
{
    [SerializeField] private List<Sprite> spritesList;
    [SerializeField] private DoorController door;
    [SerializeField] private int idSymbol = 0; // Indicate if this show the first (1) or second (2) symbol
    private Renderer _renderer = null;

    private (randomCodePicker.Symbol, randomCodePicker.Symbol) syms;
    private randomCodePicker.Symbol _symbol;
    private Color _rgbaColor = Color.white;
    
    void Start()
    {
        syms = door.GetComponentInChildren<randomCodePicker>().GETSymbols();
        _renderer = GetComponent<Renderer>();
        
        switch (idSymbol)
        {
            case 1:
                _symbol = syms.Item1;
                break;
            case 2:
                _symbol = syms.Item2;
                break;
            default:
                throw new Exception("This pannel ID doesn't exist!\nChoices are: 1 or 2");
        }

        Debug.Log(_symbol);
        _renderer.material.mainTexture = spritesList[(int)_symbol].texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
