using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Level1Controller : MonoBehaviour
{
    [SerializeField] private Color[] PossibleColors = null;
    public Color[] GetColors() => PossibleColors;
    public Color GetNextColorInSequence() => FurnaceController.GetNextColor();
    public int GetCurrentSequenceLenght() => FurnaceController.GetCurrentSequenceLenght();

    [SerializeField] private FurnaceController FurnaceController = null;
    [SerializeField] private TransportableSpawner TransportableSpawner = null;
    [SerializeField] private DoorsScript Level1Door = null;

    [Tooltip("Duration (Seconds) of items being cleared off the conveyors.")]
    [SerializeField] private float ClearItemsTimeSeconds = 3;
    [Tooltip("Duration (Seconds) of items spawning rapidly at the start of a new sequence.")]
    [SerializeField] private float FastItemSpawningTimeSeconds = 4;
    [Tooltip("Speed at wich the conveyor goes when spawning items at the start of a new sequence. Also used for clearing Items after a sequence.")]
    [SerializeField] private float MaxConveyorSpeed = 15;
    [Tooltip("Speed at wich the conveyor starts at after spawning items rapidly.")]
    [SerializeField] private float MinConveyorSpeed = 1;
    [Tooltip("Added to previous MinConveyorSpeed after a successful sequence.")]
    [SerializeField] private float ConveyorSpeedIncrement = 0.5f;
    [Tooltip("Longest delay range (Seconds) between each two items spawning back to back.")]
    [SerializeField] private Vector2 DelayBetweenItemSpawnsSecondsHighest = new Vector2(0.5f,1f);
    [Tooltip("Shortest delay range (Seconds) between each two items spawning back to back. Used for rapidly spawning items at high speed.")]
    [SerializeField] private Vector2 DelayBetweenItemSpawnsSecondsLowest = new Vector2(0.1f,0.1f);

    private float conveyorOperatingSpeed;
    private bool firstWave = false;

    public void Start()
    {
        FurnaceController.enabled = false;
        TransportableSpawner.enabled = false;
        conveyorOperatingSpeed = MinConveyorSpeed;
    }

    private void OnEnable()
    {
        Level1Door.OnDoorUnlockEvent += OnDoorUnlockEvent;
    }

    private void OnDisable()
    {
        Level1Door.OnDoorUnlockEvent -= OnDoorUnlockEvent;
    }

    private void OnDoorUnlockEvent(DoorsScript doorsScript)
    {
        StartLevel();
    }

    public void FinishLevel()
    {
        TransportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        StartCoroutine(EndLevel());
    }

    public void StartLevel()
    {
        firstWave = true;
        FurnaceController.enabled = true;
        TransportableSpawner.enabled = true;
        TransportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
    }

    public void InitiateNextSequence()
    {
        ActivateItemSpawning(false);       
        TransportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
    }

    private void ActivateItemSpawning(bool canSpawn)
    {
        TransportableSpawner.ActivateSpawning(canSpawn);
    }

    private void SetDelayBetweenItemSpawns(Vector2 delayRange)
    {
        TransportableSpawner.SetDelayBetweenSpawns(delayRange);
    }

    IEnumerator waitForItemsToClear(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    IEnumerator SpawnFreshItems(float seconds)
    {
        if (!firstWave)
        {
            yield return waitForItemsToClear(ClearItemsTimeSeconds);
        }      
        SetDelayBetweenItemSpawns(DelayBetweenItemSpawnsSecondsLowest);
        ActivateItemSpawning(true);
        yield return new WaitForSeconds(seconds);
        TransportableSpawner.SetConveyorsSpeed(conveyorOperatingSpeed);
        if (firstWave)
        {
            TransportableSpawner.SetConveyorsSpeed(conveyorOperatingSpeed);
        }
        else
        {
            firstWave = false;
            TransportableSpawner.SetConveyorsSpeed(conveyorOperatingSpeed+=ConveyorSpeedIncrement); 
        }

        SetDelayBetweenItemSpawns(DelayBetweenItemSpawnsSecondsHighest);       
    }

    IEnumerator EndLevel()
    {
        waitForItemsToClear(ClearItemsTimeSeconds);
        yield return null;
        FurnaceController.enabled = false;
        TransportableSpawner.enabled = false;
        TransportableSpawner.gameObject.SetActive(false);
    }
}
