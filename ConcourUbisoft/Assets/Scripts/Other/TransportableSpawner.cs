using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportableSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] TransportablesPrefab = null;
    [SerializeField] private Transform PointA = null;
    [SerializeField] private Transform PointB = null;
    [SerializeField] private Level1Controller Level1Controller = null;
    [SerializeField] private Vector2 DelayBetweenSpawnsInSeconds = new Vector2(0.5f, 1);
    [SerializeField] private bool CanSpawn = true;
    
    // Control conveyor speed according to needs. Ex: Slow conveyor speed might need a speed boost when items are spawned the first time in order to avoid item drought.
    [SerializeField] private Conveyor[] conveyors = null;

    private float lastSpawnTime = 0.0f;
    private float currentDelay = 0.0f;

    private int sequenceIndex = 0;

    private System.Random _random = new System.Random(0);

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (CanSpawn &&Time.time - lastSpawnTime > currentDelay)
        {
            Spawn();
            lastSpawnTime = Time.time;
            currentDelay = (float)_random.NextDouble()*(DelayBetweenSpawnsInSeconds.y - DelayBetweenSpawnsInSeconds.x) + DelayBetweenSpawnsInSeconds.x;
        }
    }

    private void Spawn()
    {
        Color[] possibleColors = Level1Controller.GetColors();

        GameObject randomPrefab = TransportablesPrefab[_random.Next(0, TransportablesPrefab.Length)];

        Vector3 randomPoint = PointA.position + _random.Next(0, 100) / 100.0f * (PointB.position - PointA.position);

        GameObject transportable;

        if (sequenceIndex > Level1Controller.GetCurrentSequenceLenght())
        {
            foreach (var t in TransportablesPrefab)
            {
                if (t.GetComponent<TransportableByConveyor>().GetType() == Level1Controller.GetNextTypeInSequence())
                {
                    transportable = Instantiate(t, randomPoint, Quaternion.identity);
                    transportable.gameObject.GetComponent<TransportableByConveyor>().Color = Level1Controller.GetNextColorInSequence();
                    break;
                }
            }
            sequenceIndex = 0;
        }
        else
        { 
            transportable = Instantiate(randomPrefab, randomPoint, Quaternion.identity);
            Color randomColor = possibleColors[_random.Next(0, possibleColors.Length)];
            transportable.gameObject.GetComponent<TransportableByConveyor>().Color = randomColor;
            sequenceIndex++;
        }
        
    }

    public void ActivateSpawning(bool canSpawn)
    {
        CanSpawn = canSpawn;
    }

    public void SetConveyorsSpeed(float speed)
    {
        foreach (Conveyor c in conveyors)
        {
            c.SetSpeed(speed);
        }
    }

    public void SetDelayBetweenSpawns(Vector2 delay)
    {
        DelayBetweenSpawnsInSeconds = delay;
    }

}
