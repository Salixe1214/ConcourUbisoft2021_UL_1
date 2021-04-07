using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnLineConveyor : MonoBehaviour
{
    [SerializeField] private Collider _colliderToSpawnObjectOn = null;
    [SerializeField] private int _numberOfTry = 0;
    [SerializeField] private float _padding = 0;
    [SerializeField] private Vector2 _objectSpace = new Vector2();

    private System.Random _random;
    private GameController _gameController = null;

    private void Awake()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _random = new System.Random(_gameController.Seed);
    }

    public IEnumerable<Bounds> GetSpawnPosition()
    {
        List<Bounds> solutions = new List<Bounds>();

        Vector3 center = _colliderToSpawnObjectOn.bounds.center;
        Vector3 extents = _colliderToSpawnObjectOn.bounds.extents;

        for (int i = 0; i < _numberOfTry; ++i)
        {
            float yPosition = center.y + 1;
            float minX = center.x - extents.x + _padding;
            float maxX = center.x + extents.x - _padding;
            float xPosition = (float)_random.NextDouble()*(maxX - minX) + minX;

            float minZ = center.z - extents.z + _padding;
            float maxZ = center.z + extents.z - _padding;
            float zPosition = (float)_random.NextDouble() * (maxZ - minZ) + minZ;

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
