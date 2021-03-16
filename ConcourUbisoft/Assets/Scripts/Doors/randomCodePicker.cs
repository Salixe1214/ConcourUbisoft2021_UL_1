using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doors;
using UnityEngine;
using Random = UnityEngine.Random;

public class randomCodePicker : MonoBehaviour
{

    private (DoorCode.Symbol, DoorCode.SymbolColor) _firstSymbol;
    private (DoorCode.Symbol, DoorCode.SymbolColor) _secondSymbol;
    private List<DoorController.Direction> _sequence = new List<DoorController.Direction>();
    
    // When the object awake, it randomly compose a combination of two different symbols of two different colors
    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        #region choosing Symbol
        
        // Choice of the first symbol
        var symbolValues = Enum.GetValues(typeof(DoorCode.Symbol)); // List of the symbols
        int firstSymbolIndex = Random.Range(0, symbolValues.Length); // Random index in this list
        Debug.Log("Index1: " + firstSymbolIndex);
        DoorCode.Symbol randomSymbol1 = (DoorCode.Symbol) symbolValues.GetValue(firstSymbolIndex); // Expliciting this symbol
        
        // We can't choose this symbol again
        int secondSymbolIndex = firstSymbolIndex;
        
        int infiniteLoopProtection = 1000;
        while (secondSymbolIndex == firstSymbolIndex)
        {
            secondSymbolIndex = Random.Range(0, symbolValues.Length);
            
            // Te ensure there is no infinite loop
            infiniteLoopProtection = infiniteLoopProtection - 1;
            if(infiniteLoopProtection <= 0)
            {
                throw new Exception("An error occured while selecting the second Symbol");
            }
        }
        
        DoorCode.Symbol randomSymbol2 = (DoorCode.Symbol) symbolValues.GetValue(secondSymbolIndex);

        #endregion


        #region cgoosing Color

        // Choice of the first color
        var colorValues = Enum.GetValues(typeof(DoorCode.SymbolColor)); // List of the colors
        int firstColorIndex = Random.Range(0, colorValues.Length); // Random index in this list
        DoorCode.SymbolColor randomColor1 = (DoorCode.SymbolColor) colorValues.GetValue(firstColorIndex); // Expliciting this symbol
        
        // We can't choose this color again
        int secondColorIndex = firstColorIndex;
        
        infiniteLoopProtection = 1000;
        while (secondColorIndex == firstColorIndex)
        {
            secondColorIndex = Random.Range(0, colorValues.Length);
            
            // Te ensure there is no infinite loop
            infiniteLoopProtection = infiniteLoopProtection - 1;
            if(infiniteLoopProtection <= 0)
            {
                throw new Exception("An error occured while selecting the second Symbol");
            }
        }
        
        DoorCode.SymbolColor randomColor2 = (DoorCode.SymbolColor) colorValues.GetValue(secondColorIndex);

        #endregion
        
        
        // Initializing the global variables
        _firstSymbol = (randomSymbol1, randomColor1);
        _secondSymbol = (randomSymbol2, randomColor2);
        _sequence.AddRange(DoorCode.GetSymbolCode(_firstSymbol.Item2,_firstSymbol.Item1));
        _sequence.AddRange(DoorCode.GetSymbolCode(_secondSymbol.Item2, _secondSymbol.Item1));
        string a = "";
        foreach (var i in _sequence)
        {
            a = a + i + " - ";
        }
        Debug.Log("Sequence of " + gameObject.name + ": " + a);
        Debug.Log(("Symbol1: " + _firstSymbol));
        Debug.Log(("Symbol2: " + _secondSymbol));

    }

    #region getters

    public ((DoorCode.Symbol, DoorCode.SymbolColor),(DoorCode.Symbol, DoorCode.SymbolColor)) GETSymbols()
    {
        return (_firstSymbol, _secondSymbol);
    }

    public List<DoorController.Direction> GetSequence()
    {
        return _sequence;
    }

    #endregion
    
}
