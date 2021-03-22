using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport
{
    public class GridSystem
    {
        private Vector2Int _size = Vector2Int.one;
        private Rect[] _rects = {};

        #region Grid
    
        public void Init(int elementNbr)
        {
            _size = CalculateGridSize(elementNbr);
            _rects = CalculateGrid(elementNbr, _size);
        }
        
        public Vector2Int CalculateGridSize(int size)
        {
            float rLenght = Mathf.Sqrt(size);

            return new Vector2Int(Mathf.CeilToInt(rLenght), Mathf.RoundToInt(rLenght));
        }

        public Rect[] CalculateGrid(int len, Vector2Int size)
        {
            Rect[] rects = new Rect[len];
            Vector2 cell = new Vector2(1f / size.x, 1f / size.y);
            float maxY = 1f - cell.y;

            for (int i = 0; i < len; i++)
            {
                float x = (float) i % size.x;
                rects[i].Set(x * cell.x, maxY - ((float)i - x) / size.y * cell.y,
                    cell.x, cell.y);
            }

            return rects;
        }
    
        public void Grid(IEnumerable<Camera> cams)
        {
            foreach ((Camera camera, int index) in cams.Select((value, i) => ( value, i )))
            {
                camera.rect = _rects[index];
            }
        }

        public void Grid(IEnumerable<Button> gameObjects)
        {
            foreach ((Button go, int index) in gameObjects.Select((value, i) => ( value, i )))
            {
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    _rects[index].width * Screen.width,
                    _rects[index].height * Screen.height);
                go.GetComponent<RectTransform>().position = new Vector2(
                    _rects[index].x * Screen.width + _rects[index].width * Screen.width / 2, 
                    _rects[index].y * Screen.height + _rects[index].height * Screen.height / 2);
            }
        }

        #endregion

        #region Getter Setter

        public Vector2Int GetGridSize()
        {
            return _size;
        }

        #endregion
    }
}
