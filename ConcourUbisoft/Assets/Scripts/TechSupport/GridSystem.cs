using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TechSupport
{
    public class GridSystem
    {
        private Vector2Int _size = Vector2Int.one;
        private Rect[] _rects = new Rect[]{};

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

        #endregion

        #region Getter Setter

        public Vector2Int GetGridSize()
        {
            return _size;
        }

        #endregion
    }
}
