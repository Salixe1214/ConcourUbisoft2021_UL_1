using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class randomCodePicker : MonoBehaviour
{
    // Those are the colors possible for the syboles
    // They're used mainly in switches
    public enum SymbolColor
    {
        Blue,
        Green,
        Red,
        Yellow,
        Purple
    }

    // Those are the types of symbols, to make switches the easy way
    public enum Symbol
    {
        One = 0,
        Two = 1,
        Three = 2
    }

    [SerializeField] private int _seedTemp = 0;

    private (Symbol, SymbolColor) _firstSymbol;
    private (Symbol, SymbolColor) _secondSymbol;
    private List<DoorController.Direction> _sequence = new List<DoorController.Direction>();
    private Random _random;

    // When the object awake, it randomly compose a combination of two different symbols of two different colors
    private void Awake()
    {
        _random = new Random(_seedTemp);

        #region choosing Symbol

        // Choice of the first symbol
        var symbolValues = Enum.GetValues(typeof(Symbol)); // List of the symbols
        int firstSymbolIndex = _random.Next(0, symbolValues.Length); // Random index in this list
        Symbol randomSymbol1 = (Symbol)symbolValues.GetValue(firstSymbolIndex); // Expliciting this symbol

        // We can't choose this symbol again
        int secondSymbolIndex = firstSymbolIndex;

        int infiniteLoopProtection = 1000;
        while (secondSymbolIndex == firstSymbolIndex)
        {
            secondSymbolIndex = _random.Next(0, symbolValues.Length);

            // Te ensure there is no infinite loop
            infiniteLoopProtection = infiniteLoopProtection - 1;
            if (infiniteLoopProtection <= 0)
            {
                throw new Exception("An error occured while selecting the second Symbol");
            }
        }

        Symbol randomSymbol2 = (Symbol)symbolValues.GetValue(secondSymbolIndex);

        #endregion


        #region cgoosing Color

        // Choice of the first color
        var colorValues = Enum.GetValues(typeof(SymbolColor)); // List of the colors
        int firstColorIndex = _random.Next(0, colorValues.Length); // Random index in this list
        SymbolColor randomColor1 = (SymbolColor) colorValues.GetValue(firstColorIndex); // Expliciting this symbol

        // We can't choose this color again
        int secondColorIndex = firstColorIndex;

        infiniteLoopProtection = 1000;
        while (secondColorIndex == firstColorIndex)
        {
            secondColorIndex = _random.Next(0, colorValues.Length);

            // Te ensure there is no infinite loop
            infiniteLoopProtection = infiniteLoopProtection - 1;
            if (infiniteLoopProtection <= 0)
            {
                throw new Exception("An error occured while selecting the second Symbol");
            }
        }

        SymbolColor randomColor2 = (SymbolColor)colorValues.GetValue(secondColorIndex);

        #endregion


        // Initializing the global variables
        _firstSymbol = (randomSymbol1, randomColor1);
        _secondSymbol = (randomSymbol2, randomColor2);
        _sequence.AddRange(getSymbolCode(_firstSymbol.Item2, _firstSymbol.Item1));
        _sequence.AddRange(getSymbolCode(_secondSymbol.Item2, _secondSymbol.Item1));
        string a = "";
        foreach (var i in _sequence)
        {
            a = a + i + " - ";
        }
        Debug.Log("RandomCodePicker: Sequence of " + gameObject.name + ": " + a);
        Debug.Log(("RandomCodePicker: Symbol1: " + _firstSymbol));
        Debug.Log(("RandomCodePicker: Symbol2: " + _secondSymbol));

    }

    // This class return the sequence associated to a symbol and a color
    private List<DoorController.Direction> getSymbolCode(SymbolColor pColor, Symbol pSymbol)
    {
        List<DoorController.Direction> dirList = new List<DoorController.Direction>();

        switch (pColor)
        {
            case SymbolColor.Blue:
                switch (pSymbol)
                {
                    case Symbol.One:
                        dirList.Add(DoorController.Direction.Up);
                        dirList.Add(DoorController.Direction.Up);
                        dirList.Add(DoorController.Direction.Left);
                        break;
                    case Symbol.Two:
                        dirList.Add(DoorController.Direction.Bottom);
                        dirList.Add(DoorController.Direction.Bottom);
                        dirList.Add(DoorController.Direction.Bottom);
                        break;
                    case Symbol.Three:
                        dirList.Add(DoorController.Direction.Up);
                        dirList.Add(DoorController.Direction.Left);
                        dirList.Add(DoorController.Direction.Right);
                        break;
                    default:
                        dirList.Clear();
                        break;
                }
                break;
            case SymbolColor.Green:
                switch (pSymbol)
                {
                    case Symbol.One:
                        dirList.Add(DoorController.Direction.Left);
                        dirList.Add(DoorController.Direction.Left);
                        dirList.Add(DoorController.Direction.Left);
                        break;
                    case Symbol.Two:
                        dirList.Add(DoorController.Direction.Bottom);
                        dirList.Add(DoorController.Direction.Up);
                        dirList.Add(DoorController.Direction.Up);
                        break;
                    case Symbol.Three:
                        dirList.Add(DoorController.Direction.Bottom);
                        dirList.Add(DoorController.Direction.Bottom);
                        dirList.Add(DoorController.Direction.Bottom);
                        break;
                    default:
                        dirList.Clear();
                        break;
                }
                break;
            case SymbolColor.Red:
                switch (pSymbol)
                {
                    case Symbol.One:
                        dirList.Add(DoorController.Direction.Right);
                        dirList.Add(DoorController.Direction.Left);
                        dirList.Add(DoorController.Direction.Up);
                        break;
                    case Symbol.Two:
                        dirList.Add(DoorController.Direction.Left);
                        dirList.Add(DoorController.Direction.Right);
                        dirList.Add(DoorController.Direction.Right);
                        break;
                    case Symbol.Three:
                        dirList.Add(DoorController.Direction.Right);
                        dirList.Add(DoorController.Direction.Up);
                        dirList.Add(DoorController.Direction.Bottom);
                        break;
                    default:
                        dirList.Clear();
                        break;
                }
                break;
            case SymbolColor.Yellow:
                switch (pSymbol)
                {
                    case Symbol.One:
                        dirList.Add(DoorController.Direction.Left);
                        dirList.Add(DoorController.Direction.Right);
                        dirList.Add(DoorController.Direction.Left);
                        break;
                    case Symbol.Two:
                        dirList.Add(DoorController.Direction.Right);
                        dirList.Add(DoorController.Direction.Right);
                        dirList.Add(DoorController.Direction.Right);
                        break;
                    case Symbol.Three:
                        dirList.Add(DoorController.Direction.Up);
                        dirList.Add(DoorController.Direction.Left);
                        dirList.Add(DoorController.Direction.Left);
                        break;
                    default:
                        dirList.Clear();
                        break;
                }
                break;
            case SymbolColor.Purple:
                switch (pSymbol)
                {
                    case Symbol.One:
                        dirList.Add(DoorController.Direction.Up);
                        dirList.Add(DoorController.Direction.Up);
                        dirList.Add(DoorController.Direction.Right);
                        break;
                    case Symbol.Two:
                        dirList.Add(DoorController.Direction.Right);
                        dirList.Add(DoorController.Direction.Bottom);
                        dirList.Add(DoorController.Direction.Right);
                        break;
                    case Symbol.Three:
                        dirList.Add(DoorController.Direction.Bottom);
                        dirList.Add(DoorController.Direction.Bottom);
                        dirList.Add(DoorController.Direction.Up);
                        break;
                    default:
                        dirList.Clear();
                        break;
                }
                break;
            default:
                dirList.Clear();
                break;
        }

        return dirList;
    }

    #region getters

    public ((Symbol, SymbolColor), (Symbol, SymbolColor)) GETSymbols()
    {
        return (_firstSymbol, _secondSymbol);
    }

    public List<DoorController.Direction> GetSequence()
    {
        return _sequence;
    }

    #endregion

}
