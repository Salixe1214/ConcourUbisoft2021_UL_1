using System.Collections.Generic;
using UnityEngine;

namespace TechSupport.Surveillance
{
    public class GridSystem
    {
        private Vector2Int _size = Vector2Int.one;

        #region Grid
    
        public Vector2Int CalculateGridSize(int size)
        {
            float rLenght = Mathf.Sqrt(size);

            return new Vector2Int(Mathf.CeilToInt(rLenght), Mathf.RoundToInt(rLenght));
        }
    
        public void Grid(List<Camera> cams, Vector2Int size)
        {
            float x = 0;
            float y = 0;

            for (int i = 0; i < cams.Count; i++)
            {
                cams[i].rect = new Rect(x * (1f / size.x), y * (1f / size.y),
                    1f / size.x, 1f / size.y);
                x++;
                if (x % size.x == 0)
                {
                    y++;
                    x = 0;
                }
            }
        }
    
        #endregion

        #region Getter Setter

        public void SearchGridSize(int elementNbr)
        {
            _size = CalculateGridSize(elementNbr);
        }

        public Vector2Int GetGridSize()
        {
            return _size;
        }

        #endregion
    }
}
