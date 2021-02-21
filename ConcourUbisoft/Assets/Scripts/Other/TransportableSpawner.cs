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
    
    // Control conveyor speed according to needs. Ex: Slow conveyor speed might need a speed boost when items are spawned the first time in order to avoid item drought.
    [SerializeField] private Conveyor[] conveyors = null;

    private float lastSpawnTime = 0.0f;
    private float currentDelay = 0.0f;

    private int sequenceIndex = 0;

    private void Update()
    {
        if (Time.time - lastSpawnTime > currentDelay)
        {
            Spawn();
            lastSpawnTime = Time.time;
            currentDelay = Random.Range(DelayBetweenSpawnsInSeconds.x, DelayBetweenSpawnsInSeconds.y);
        }
    }

    //TODO : Make sure that required items are available. Make them respawn if destroyed. DONE
    //TODO : Speed up conveyors when items spawn the first time.
    //TODO : Add Speed when players progress. After each sequence?
    //TODO : Clear Items after sequence? Speed up to clear.
    private void Spawn()
    {
        
        Color[] possibleColors = Level1Controller.GetColors();

        GameObject randomPrefab = TransportablesPrefab[Random.Range(0, TransportablesPrefab.Length)];
        
        Vector3 randomPoint = PointA.position + Random.Range(0, 100) / 100.0f * (PointB.position - PointA.position);

        GameObject transportable = Instantiate(randomPrefab, randomPoint, Quaternion.identity);
        
        
        if (sequenceIndex >= Level1Controller.GetCurrentSequenceLenght())
        {
            transportable.gameObject.GetComponent<TransportableByConveyor>().Color = Level1Controller.GetNextColorInSequence();
            sequenceIndex = 0;
        }
        else
        {
            Color randomColor = possibleColors[Random.Range(0, possibleColors.Length)];
            transportable.gameObject.GetComponent<TransportableByConveyor>().Color = randomColor;
            sequenceIndex++;
        }
        
    }
}
