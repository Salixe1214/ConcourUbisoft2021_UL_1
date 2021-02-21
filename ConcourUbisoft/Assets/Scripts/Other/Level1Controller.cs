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

    [SerializeField] private float ClearItemsTimeSeconds = 3;
    [SerializeField] private float FastItemSpawningTimeSeconds = 4;
    [SerializeField] private float MaxConveyorSpeed = 15;
    [SerializeField] private float MinConveyorSpeed = 1;
    [SerializeField] private float ConveyorSpeedIncrement = 0.5f;
    [SerializeField] private Vector2 DelayBetweenItemSpawnsSeconds = new Vector2(0.5f,1f);
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

        SetDelayBetweenItemSpawns(DelayBetweenItemSpawnsSeconds);       
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
