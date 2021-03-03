using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level1Controller : MonoBehaviour
{
    [SerializeField] private Color[] PossibleColors = null;
    public Color[] GetColors() => PossibleColors;
    public Color GetNextColorInSequence() => FurnaceController.GetNextColor();
    public int GetCurrentSequenceLenght() => FurnaceController.GetCurrentSequenceLenght();

    [SerializeField] private FurnaceController FurnaceController = null;
    [SerializeField] private TransportableSpawner TransportableSpawner = null;
    [SerializeField] private DoorsScript Level1Door = null;
    [SerializeField] private Camera AreaCamera = null;

    [Tooltip("Duration (Seconds) of items being cleared off the conveyors.")]
    [SerializeField] private float ClearItemsTimeSeconds = 3;
    [Tooltip("Duration (Seconds) of items spawning rapidly at the start of a new sequence.")]
    [SerializeField] private float FastItemSpawningTimeSeconds = 4;
    [Tooltip("Speed at wich the conveyor goes when spawning items at the start of a new sequence. Also used for clearing Items after a sequence.")]
    [SerializeField] private float MaxConveyorSpeed = 15;
    [Tooltip("Speed at wich the conveyor starts at after spawning items rapidly.")]
    [SerializeField] private float MinConveyorSpeed = 1.5f;
    [Tooltip("Added to previous MinConveyorSpeed after a successful sequence.")]
    [SerializeField] private float ConveyorSpeedIncrement = 0.2f;
    [Tooltip("Longest delay range (Seconds) between each two items spawning back to back.")]
    [SerializeField] private Vector2 DelayBetweenItemSpawnsSecondsHighest = new Vector2(1f,1.5f);
    [Tooltip("Shortest delay range (Seconds) between each two items spawning back to back. Used for rapidly spawning items at high speed.")]
    [SerializeField] private Vector2 DelayBetweenItemSpawnsSecondsLowest = new Vector2(0.1f,0.2f);
    [Tooltip("Intensity of the AreaCamera Shake Effect")]
    [SerializeField] private float cameraShakeForce = 0.3f;
    [Tooltip("Duration (Seconds) of the AreaCamera Shake effect.")]
    [SerializeField] private float cameraShakeDurationSeconds = 0.2f;

    private float conveyorOperatingSpeed;
    private bool firstWave = false;
    private Vector3 cameraOriginalPosition;
    private bool cameraMustShake = false;

    public void Start()
    {
        FurnaceController.GenerateNewColorSequences(PossibleColors);
        FurnaceController.enabled = false;
        TransportableSpawner.enabled = false;
        if (Level1Door == null)
            StartLevel();
        conveyorOperatingSpeed = MinConveyorSpeed;
        cameraOriginalPosition = AreaCamera.transform.position;
    }

    private void Update()
    {
        if (cameraMustShake)
        {
            AreaCamera.transform.position = cameraOriginalPosition + Random.insideUnitSphere * cameraShakeForce;
        }
    }

    private void OnEnable()
    {
        if (Level1Door != null)
            Level1Door.OnDoorUnlockEvent += OnDoorUnlockEvent;

        FurnaceController.WhenFurnaceConsumedAll += FinishLevel;
        FurnaceController.WhenFurnaceConsumeAWholeSequenceWithoutFinishing += InitiateNextSequence;
        FurnaceController.WhenFurnaceConsumeWrong += ShakeCamera;
    }

    private void OnDisable()
    {
        if (Level1Door != null)
            Level1Door.OnDoorUnlockEvent -= OnDoorUnlockEvent;

        FurnaceController.WhenFurnaceConsumedAll -= FinishLevel;
        FurnaceController.WhenFurnaceConsumeAWholeSequenceWithoutFinishing -= InitiateNextSequence;
        FurnaceController.WhenFurnaceConsumeWrong -= ShakeCamera;
    }

    private void OnDoorUnlockEvent(DoorsScript doorsScript)
    {
        Debug.Log("Door Unlocked");
        StartLevel();
    }

    public void FinishLevel()
    {
        TransportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        StartCoroutine(EndLevel());
    }

    public void StartLevel()
    {
        Debug.Log("StartLevel");
        firstWave = true;
        FurnaceController.enabled = true;
        TransportableSpawner.enabled = true;
        ActivateItemSpawning(false);
        TransportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        Debug.Log("ConveyorSpeed Max");
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
    }

    public void InitiateNextSequence()
    {
        ActivateItemSpawning(false);       
        TransportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
    }

    public void ShakeCamera()
    {
        StartCoroutine(StartCameraShake(cameraShakeDurationSeconds));
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
        Debug.Log("SpawnFreshItems");
        if (!firstWave)
        {
            Debug.Log("ClearItems");
            yield return waitForItemsToClear(ClearItemsTimeSeconds);
        }      
        SetDelayBetweenItemSpawns(DelayBetweenItemSpawnsSecondsLowest);
        Debug.Log("Lowest Spawning Delay");
        ActivateItemSpawning(true);
        yield return new WaitForSeconds(seconds);
        if (firstWave)
        {
            TransportableSpawner.SetConveyorsSpeed(conveyorOperatingSpeed);
            Debug.Log("ConveyorSpeed Normal");
            firstWave = false;
        }
        else
        {            
            TransportableSpawner.SetConveyorsSpeed(conveyorOperatingSpeed+=ConveyorSpeedIncrement); 
            Debug.Log("ConveyorSpeed Normal");
        }
        SetDelayBetweenItemSpawns(DelayBetweenItemSpawnsSecondsHighest);    
        Debug.Log("Highest Spawning Delay");
    }

    IEnumerator EndLevel()
    {
        waitForItemsToClear(ClearItemsTimeSeconds);
        yield return null;
        FurnaceController.enabled = false;
        TransportableSpawner.enabled = false;
        TransportableSpawner.gameObject.SetActive(false);
    }
    
    IEnumerator StartCameraShake(float duration)
    {
        cameraMustShake = true;
        yield return new WaitForSeconds(duration);
        cameraMustShake = false;
        AreaCamera.transform.position = cameraOriginalPosition;
    }
}