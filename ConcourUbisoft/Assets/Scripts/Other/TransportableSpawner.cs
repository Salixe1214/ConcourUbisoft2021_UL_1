using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Arm;
using Other;
using Photon.Pun;
using UnityEngine;

public class TransportableSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] TransportablesPrefab = null;
    [SerializeField] private Transform PointA = null;
    [SerializeField] private Transform PointB = null;
    [SerializeField] public GameObject LevelControl = null;
    [SerializeField] private Vector2 DelayBetweenSpawnsInSeconds = new Vector2(0.5f, 1);
    [SerializeField] private bool CanSpawn = false;

    // Control conveyor speed according to needs. Ex: Slow conveyor speed might need a speed boost when items are spawned the first time in order to avoid item drought.
    [SerializeField] private Conveyor[] conveyors = null;

    private NetworkController _networkController = null;

    private float lastSpawnTime = 0.0f;
    private float currentDelay = 0.0f;
    private int sequenceIndex = 0;
    private LevelController levelController;
    private System.Random _random = new System.Random(0);
    private PickableType[] currentSequenceTypes;
    private Color[] currentSequenceColors;

    public bool canSpawnNextRequiredItem =false;
    public event Action requiredItemHasSpawned;

    private void Start()
    {
        Debug.Log("Calling Start Spawners");
        levelController = LevelControl.GetComponent<LevelController>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
    }

    private void Update()
    {
        if(_networkController.GetLocalRole() == GameController.Role.Technician)
        {
            if (CanSpawn && Time.time - lastSpawnTime > currentDelay)
            {
                Spawn();
                lastSpawnTime = Time.time;
                currentDelay = (float)_random.NextDouble() * (DelayBetweenSpawnsInSeconds.y - DelayBetweenSpawnsInSeconds.x) + DelayBetweenSpawnsInSeconds.x;
            }
        }
    }

    private void Spawn()
    {
        Color[] possibleColors = levelController.GetColors();

        GameObject randomPrefab = TransportablesPrefab[_random.Next(0, TransportablesPrefab.Length)];

        Vector3 randomPoint = PointA.position + _random.Next(0, 100) / 100.0f * (PointB.position - PointA.position);

        GameObject transportable;
        
        
        if (canSpawnNextRequiredItem)
        {
            foreach (var t in TransportablesPrefab)
            {
                if (sequenceIndex >= levelController.GetCurrentSequenceLenght())
                {
                    sequenceIndex = levelController.GetCurrentSequenceIndex();
                }

                if (t.GetComponent<Pickable>().GetType() == currentSequenceTypes[sequenceIndex])
                {
                    transportable = PhotonNetwork.Instantiate(t.name, randomPoint, Quaternion.identity);
                    transportable.GetComponent<Arm.Pickable>().Color = currentSequenceColors[sequenceIndex];
                    Debug.Log("INVOKED");
                    requiredItemHasSpawned?.Invoke();
                    sequenceIndex++;
                    break;
                }
            }
        }
        else
        {
            Color randomColor = possibleColors[_random.Next(0, possibleColors.Length)];
            transportable = PhotonNetwork.Instantiate(randomPrefab.name, randomPoint, Quaternion.identity);
            transportable.gameObject.GetComponent<Pickable>().Color = randomColor;
        }
        
    }

    public void ActivateSpawning(bool canSpawn)
    {
        CanSpawn = canSpawn;
        if (canSpawn)
        {
            currentSequenceTypes = levelController.GetAllNextItemTypes(); 
            currentSequenceColors = levelController.GetAllNextItemColors();
            sequenceIndex = 0;
        }
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
