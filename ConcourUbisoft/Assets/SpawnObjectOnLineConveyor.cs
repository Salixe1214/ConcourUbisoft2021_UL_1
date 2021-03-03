using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnLineConveyor : MonoBehaviour
{
    [SerializeField] private Collider _colliderToSpawnObjectOn = null;
    [SerializeField] private int _numberOfTry = 0;
    [SerializeField] private float _padding = 0;
    [SerializeField] private Vector2 _objectSpace = new Vector2();

    public IEnumerable<Bounds> GetSpawnPosition()
    {
        List<Bounds> solutions = new List<Bounds>();

        Vector3 center = _colliderToSpawnObjectOn.bounds.center;
        Vector3 extents = _colliderToSpawnObjectOn.bounds.extents;

        for (int i = 0; i < _numberOfTry; ++i)
        {
            float yPosition = center.y + 1;
            float xPosition = Random.Range(center.x - extents.x + _padding, center.x + extents.x - _padding);
            float zPosition = Random.Range(center.z - extents.z + _padding, center.z + extents.z - _padding);

            bool intersect = false;
            Bounds possibleSolution = new Bounds(new Vector3(xPosition, yPosition, zPosition), new Vector3(_objectSpace.x, 1, _objectSpace.y));
            foreach (Bounds solution in solutions)
            {
                if (possibleSolution.Intersects(solution))
                {
                    intersect = true;
                }
            }

            if (!intersect)
            {
                solutions.Add(possibleSolution);
            }
        }

        return solutions;
    }
}
