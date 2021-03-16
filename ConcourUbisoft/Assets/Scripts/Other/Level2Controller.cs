using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEngine;

public class Level2Controller : MonoBehaviour , LevelController
{
    [SerializeField] private GameObject[] _transportablesPrefab = null;
    [SerializeField] private Color[] _possibleColors = null;
    [SerializeField] private SpawnObjectOnLineConveyor[] _spawners = null;
    [SerializeField] private FurnaceController _furnace = null;

    [SerializeField] private TransportableSpawner Spawner;
    
    public Color[] GetColors() => _possibleColors;
    public Color GetNextColorInSequence() => _furnace.GetNextColor();
    public int GetCurrentSequenceLenght() => _furnace.GetCurrentSequenceLenght();
    public TransportableType GetNextTypeInSequence() => _furnace.GetNextItemType();
    
    private void Start()
    {
        _furnace.GenerateNewColorSequences(_possibleColors);
        //SpawnObjects();
        Spawner.ActivateSpawning(true);
    }

    public void SpawnObjects()
    {
        List<Bounds> solutions = new List<Bounds>();
        foreach(SpawnObjectOnLineConveyor spawner in _spawners)
        {
            solutions.AddRange(spawner.GetSpawnPosition());
        }

        if(solutions.Count >= _furnace.GetCurrentSequenceLenght())
        {

            for(int i = 0; i < _furnace.GetCurrentSequenceLenght(); ++i)
            {
                int solutionIndex = Random.Range(0, solutions.Count);

                SpawnObject(solutions[solutionIndex], _furnace.GetNextColor());

                solutions.RemoveAt(solutionIndex);
            }

            foreach (Bounds solution in solutions)
            {
                SpawnObject(solution, _possibleColors[Random.Range(0, _possibleColors.Length)]);
            }
        }
    }

    private void SpawnObject(Bounds solution, Color color)
    {
        GameObject randomPrefab = _transportablesPrefab[Random.Range(0, _transportablesPrefab.Length)];
        GameObject transportable = Instantiate(randomPrefab, solution.center, Quaternion.identity);
        transportable.gameObject.GetComponent<TransportableByConveyor>().Color = color; 
    }
}
