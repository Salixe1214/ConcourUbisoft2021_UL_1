using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySymbol : MonoBehaviour
{
    [SerializeField] private List<Sprite> spritesList; // List of textures (sprites of the symbols) in order
    [SerializeField] private DoorController door; // The associated door
    [SerializeField] private int idSymbol = 0; // Indicate if this shof the first (1) or second (2) symbol
    private SpriteRenderer sprRend = null; // The sprite renderer

    private ((randomCodePicker.Symbol, randomCodePicker.SymbolColor),(randomCodePicker.Symbol, randomCodePicker.SymbolColor)) syms;
    private randomCodePicker.Symbol _symbol;
    private randomCodePicker.SymbolColor _color;
    private Color _rgbaColor;
    
    void Start()
    {
        syms = door.GetComponentInChildren<randomCodePicker>().GETSymbols();
        sprRend = GetComponent<SpriteRenderer>();
        sprRend.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        
        switch (idSymbol)
        {
            case 1:
                _symbol = syms.Item1.Item1;
                _color = syms.Item1.Item2;
                break;
            case 2:
                _symbol = syms.Item2.Item1;
                _color = syms.Item2.Item2;
                break;
            default:
                throw new Exception("This pannel ID doesn't exist!\nChoices are: 1 or 2");
        }

        sprRend.sprite = spritesList[(int)_symbol];

        switch (_color)
        {
            case randomCodePicker.SymbolColor.Blue:
                _rgbaColor = Color.blue;
                break;
            case randomCodePicker.SymbolColor.Green:
                _rgbaColor = Color.green;
                break;
            case randomCodePicker.SymbolColor.Purple:
                _rgbaColor = Color.magenta;
                break;
            case randomCodePicker.SymbolColor.Red:
                _rgbaColor = Color.red;
                break;
            case randomCodePicker.SymbolColor.Yellow:
                _rgbaColor = Color.yellow;
                break;
            default:
                _rgbaColor = Color.black;
                break;
        }

        sprRend.color = _rgbaColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
