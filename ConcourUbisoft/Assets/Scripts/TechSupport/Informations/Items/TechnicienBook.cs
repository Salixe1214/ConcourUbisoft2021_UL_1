using System.Collections.Generic;
using Doors;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Utils;

namespace TechSupport.Informations.Items
{
    public class TechnicienBook : InformationItem
    {
        private AccordionElement _accordionElement;
        
        private readonly List<(DoorCode.Symbol, DoorCode.SymbolColor)> _content =
            new List<(DoorCode.Symbol, DoorCode.SymbolColor)>()
            {
                (DoorCode.Symbol.One, DoorCode.SymbolColor.Blue),
                (DoorCode.Symbol.One, DoorCode.SymbolColor.Red),
                (DoorCode.Symbol.One, DoorCode.SymbolColor.Green),
                (DoorCode.Symbol.One, DoorCode.SymbolColor.Yellow),
                (DoorCode.Symbol.One, DoorCode.SymbolColor.Purple),
                (DoorCode.Symbol.Two, DoorCode.SymbolColor.Blue),
                (DoorCode.Symbol.Two, DoorCode.SymbolColor.Red),
                (DoorCode.Symbol.Two, DoorCode.SymbolColor.Green),
                (DoorCode.Symbol.Two, DoorCode.SymbolColor.Yellow),
                (DoorCode.Symbol.Two, DoorCode.SymbolColor.Purple),
                (DoorCode.Symbol.Three, DoorCode.SymbolColor.Blue),
                (DoorCode.Symbol.Three, DoorCode.SymbolColor.Red),
                (DoorCode.Symbol.Three, DoorCode.SymbolColor.Green),
                (DoorCode.Symbol.Three, DoorCode.SymbolColor.Yellow),
                (DoorCode.Symbol.Three, DoorCode.SymbolColor.Purple),
            };

        private readonly Dictionary<DoorCode.SymbolColor, Color> _colors = new Dictionary<DoorCode.SymbolColor, Color>()
        {
            { DoorCode.SymbolColor.Blue, Color.blue },
            { DoorCode.SymbolColor.Green, Color.green },
            { DoorCode.SymbolColor.Purple, Color.magenta },
            { DoorCode.SymbolColor.Red, Color.red },
            { DoorCode.SymbolColor.Yellow, Color.yellow }
        };

        private readonly Dictionary<DoorController.Direction, Vector3> _directions =
            new Dictionary<DoorController.Direction, Vector3>()
            {
                {DoorController.Direction.Up, Vector3.zero},
                {DoorController.Direction.Right, new Vector3(0, 0, -90)},
                {DoorController.Direction.Bottom, new Vector3(0, 0, 180)},
                {DoorController.Direction.Left, new Vector3(0, 0, 90)}
            };

        private readonly Dictionary<DoorCode.Symbol, Sprite> _symbols;
        private readonly Sprite _arrow;
        public TechnicienBook(Dictionary<DoorCode.Symbol, Sprite> symbols, Sprite arrow)
        {
            _symbols = symbols;
            _arrow = arrow;
        }

        private Image InstantiateDirection(Transform parent, DoorController.Direction direction)
        {
            Image image = GameObjectsInstantiator.InstantiateImage(parent, _arrow, Color.black, Image.Type.Simple);

            image.transform.Rotate(_directions[direction]);
            return image;
        }

        private HorizontalLayoutGroup InstantiateLine((DoorCode.Symbol, DoorCode.SymbolColor) combination)
        {
            HorizontalLayoutGroup line = GameObjectsInstantiator.CreateHorizontalLayoutGroup();

            line.transform.SetParent(_accordionElement.transform);
            GameObjectsInstantiator.InstantiateImage(line.gameObject.transform, _symbols[combination.Item1],
                _colors[combination.Item2], Image.Type.Simple);
             GameObjectsInstantiator.InstantiateText(line.gameObject.transform, " = ").fontSize = 16;
            DoorCode.GetSymbolCode(combination.Item2, combination.Item1)
                .ForEach(d => InstantiateDirection(line.gameObject.transform, d));
            return line;
        }

        public override void Instantiate(Transform parent, Sprite backgroundSprite)
        {
            _accordionElement = GameObjectsInstantiator.InstantiateNewItem(parent, backgroundSprite);
            GameObjectsInstantiator.InstantiateHeader(_accordionElement.transform, "Livre du technicien");
            _content.ForEach(c => InstantiateLine(c));
        }

        public override void UpdateItem(InformationItem item)
        {
            throw new System.NotImplementedException();
        }

        public override void Delete()
        {
            Object.Destroy(_accordionElement);
        }
    }
}
