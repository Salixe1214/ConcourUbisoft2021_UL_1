using System.Collections.Generic;
using UnityEngine;

namespace Doors
{
    public class DoorCode : MonoBehaviour
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
            One,
            Two,
            Three
        }

        // This class return the sequence associated to a symbol and a color
        public static List<DoorController.Direction> GetSymbolCode(SymbolColor pColor, Symbol pSymbol)
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
    }
}
