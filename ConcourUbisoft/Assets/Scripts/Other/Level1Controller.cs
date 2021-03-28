using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using JetBrains.Annotations;
using Other;
using TechSupport.Informations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Level1Controller : MonoBehaviour , LevelController
{
    [SerializeField] private Color[] PossibleColors = null;
    public Color[] GetColors() => PossibleColors;
    public Color GetNextColorInSequence() => FurnaceController.GetNextColor();
    public int GetCurrentSequenceLenght() => FurnaceController.GetCurrentSequenceLenght();
    public PickableType GetNextTypeInSequence() => FurnaceController.GetNextItemType();

    [SerializeField] private FurnaceController FurnaceController = null;
    [SerializeField] private TransportableSpawner InteriorConveyorSpawner;
    [SerializeField] private TransportableSpawner ExteriorConveyorSpawner;
    [SerializeField] private Camera AreaCamera = null;
    [SerializeField] private InformationsSystem techUI;
    [SerializeField] private Sprite RobotHeadImage;
    [SerializeField] private Sprite CrateImage;
    [SerializeField] private Sprite GearImage;
    [SerializeField] private Sprite BatteryImage;
    [SerializeField] private Sprite PipeImage;

    [Tooltip("Duration (Seconds) of items being cleared off the conveyors.")]
    [SerializeField] private float ClearItemsTimeSeconds = 3;
    [Tooltip("Duration (Seconds) of items spawning rapidly at the start of a new sequence.")]
    [SerializeField] private float FastItemSpawningTimeSeconds = 4;
    [Tooltip("Speed at wich the conveyor goes when spawning items at the start of a new sequence. Also used for clearing Items after a sequence.")]
    [SerializeField] private float MaxInteriorConveyorSpeed = 15;
    [Tooltip("Speed at wich the conveyor goes when spawning items at the start of a new sequence. Also used for clearing Items after a sequence.")]
    [SerializeField] private float MaxExteriorConveyorSpeed = 15;
    [Tooltip("Speed at wich the conveyor starts at after spawning items rapidly.")]
    [SerializeField] private float MinConveyorSpeed = 1.5f;
    [Tooltip("Added to previous MinConveyorSpeed after a successful sequence.")]
    [SerializeField] private float ConveyorSpeedIncrement = 0.2f;
    [Tooltip("Longest delay range (Seconds) between each two items spawning back to back. Interior Conveyor")]
    [SerializeField] private Vector2 DelayItemSpawnsHighestInteriorConveyor = new Vector2(1f,1.5f);
    [Tooltip("Shortest delay range (Seconds) between each two items spawning back to back. Used for rapidly spawning items at high speed. Interior Conveyor")]
    [SerializeField] private Vector2 DelayItemSpawnsLowestInteriorConveyor = new Vector2(0.1f,0.2f);
    [Tooltip("Longest delay range (Seconds) between each two items spawning back to back. Exterior Conveyor")]
    [SerializeField] private Vector2 DelayItemSpawnsHighestExteriorConveyor = new Vector2(1f,1.5f);
    [Tooltip("Shortest delay range (Seconds) between each two items spawning back to back. Used for rapidly spawning items at high speed. Exterior Conveyor")]
    [SerializeField] private Vector2 DelayItemSpawnsLowestExteriorConveyor = new Vector2(0.1f,0.2f);
    [Tooltip("Intensity of the AreaCamera Shake Effect")]
    [SerializeField] private float cameraShakeForce = 0.3f;
    [Tooltip("Duration (Seconds) of the AreaCamera Shake effect.")]
    [SerializeField] private float cameraShakeDurationSeconds = 0.2f;
    [Tooltip("Duration (Seconds) before next required item is spawned.")]
    [SerializeField] private float delayBeforeNextRequiredItem = 6f;
    

    private float conveyorOperatingSpeed;
    private bool firstWave = false;
    private Vector3 cameraOriginalPosition;
    private bool cameraMustShake = false;
    private ImageLayout imageList;
    private List<Sprite> itemSprites;
    private SoundController soundController;
    private int currentListIndex;
    private List<TransportableSpawner> TransportableSpawners;

    private void Awake()
    {
        itemSprites = new List<Sprite>();
        TransportableSpawners = new List<TransportableSpawner>();
    }

    public void Start()
    {
        TransportableSpawners.Add(InteriorConveyorSpawner);
        TransportableSpawners.Add(ExteriorConveyorSpawner);
        currentListIndex = 0;
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        FurnaceController.GenerateNewColorSequences(PossibleColors);
        FurnaceController.enabled = false;
       /* foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
        {
            transportableSpawner.enabled = false;
        }*/

        InteriorConveyorSpawner.enabled = false;
        ExteriorConveyorSpawner.enabled = false;
        
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
        FurnaceController.WhenFurnaceConsumedAll.AddListener(FinishLevel);
        FurnaceController.WhenFurnaceConsumeAWholeSequenceWithoutFinishing.AddListener(InitiateNextSequence);
        FurnaceController.WhenFurnaceConsumeWrong.AddListener(ShakeCamera);
        FurnaceController.CheckItemOffList += UpdateSpriteColorInList;
        InteriorConveyorSpawner.requiredItemHasSpawned += SpawnNextRequiredItem;
        ExteriorConveyorSpawner.requiredItemHasSpawned += SpawnNextRequiredItem;
    }

    private void OnDisable()
    {
        FurnaceController.WhenFurnaceConsumedAll.RemoveListener(FinishLevel);
        FurnaceController.WhenFurnaceConsumeAWholeSequenceWithoutFinishing.RemoveListener(InitiateNextSequence);
        FurnaceController.WhenFurnaceConsumeWrong.RemoveListener(ShakeCamera);
        FurnaceController.CheckItemOffList -= UpdateSpriteColorInList;
        InteriorConveyorSpawner.requiredItemHasSpawned -= SpawnNextRequiredItem;
        ExteriorConveyorSpawner.requiredItemHasSpawned -= SpawnNextRequiredItem;
    }

    public void FinishLevel()
    {
       /* foreach(TransportableSpawner transportableSpawner in TransportableSpawners)
        {
            transportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        }*/
        SetConveyorSpeed(MaxInteriorConveyorSpeed,MaxExteriorConveyorSpeed);
        StartCoroutine(EndLevel());
    }

    public void StartLevel()
    {
        Debug.Log("StartLevel");
        soundController.PlayArea1Music();
        imageList = techUI.GetList();
        imageList.Clean();
        firstWave = true;
        FurnaceController.enabled = true;
        /*foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
        {
            transportableSpawner.enabled = true;
        }*/
        InteriorConveyorSpawner.enabled = true;
        ExteriorConveyorSpawner.enabled = true;
        Debug.Log(TransportableSpawners[0]);
        Debug.Log(TransportableSpawners[1]);
        ActivateItemSpawning(false);

        /*foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
        {
            transportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        }*/
        SetConveyorSpeed(MaxInteriorConveyorSpeed,MaxExteriorConveyorSpeed);
        
        Debug.Log("ConveyorSpeed Max");
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
    }

    private void InitiateNextSequence()
    {
        ActivateItemSpawning(false);
        soundController.PlayLevelSequenceClearedSuccessSound();

       /* foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
        {
            transportableSpawner.SetConveyorsSpeed(MaxConveyorSpeed);
        }*/
        SetConveyorSpeed(MaxInteriorConveyorSpeed,MaxExteriorConveyorSpeed);
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
    }

    private void ShakeCamera()
    {
        StartCoroutine(StartCameraShake(cameraShakeDurationSeconds));
    }

    private void ActivateItemSpawning(bool canSpawn)
    {
        /*foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
        {
            transportableSpawner.ActivateSpawning(canSpawn);
        }*/
        ExteriorConveyorSpawner.canSpawnNextRequiredItem = true;
        InteriorConveyorSpawner.ActivateSpawning(canSpawn);
        ExteriorConveyorSpawner.ActivateSpawning(canSpawn);
        
    }

    private void SetDelayBetweenItemSpawns(Vector2 delayRangeInteriorConveyor, Vector2 delayRangeExteriorConveyor)
    {
        /*foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
        {
            transportableSpawner.SetDelayBetweenSpawns(delayRange);
        }*/
        InteriorConveyorSpawner.SetDelayBetweenSpawns(delayRangeInteriorConveyor);
        ExteriorConveyorSpawner.SetDelayBetweenSpawns(delayRangeExteriorConveyor);
    }

    IEnumerator waitForItemsToClear(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    IEnumerator waitBeforeSpawningRequiredItem(int nextSpawnerIndex)
    {
        yield return new WaitForSeconds(delayBeforeNextRequiredItem);
        Debug.Log(nextSpawnerIndex + "is true");
        TransportableSpawners[nextSpawnerIndex].canSpawnNextRequiredItem = true;
        Debug.Log(TransportableSpawners[nextSpawnerIndex].canSpawnNextRequiredItem);
    }

    IEnumerator SpawnFreshItems(float seconds)
    {
        currentListIndex = 0;
        Debug.Log("SpawnFreshItems");
        if (!firstWave)
        {
            Debug.Log("ClearItems");
            imageList.Clean();
            yield return waitForItemsToClear(ClearItemsTimeSeconds);
        }      
        SetDelayBetweenItemSpawns(DelayItemSpawnsLowestInteriorConveyor,DelayItemSpawnsLowestExteriorConveyor);
        Debug.Log("Lowest Spawning Delay");
        
        setItemsImageList();
        ActivateItemSpawning(true);
        yield return new WaitForSeconds(seconds);
        SetDelayBetweenItemSpawns(DelayItemSpawnsHighestInteriorConveyor,DelayItemSpawnsHighestExteriorConveyor);
        if (firstWave)
        {
            /*foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
            {
                transportableSpawner.SetConveyorsSpeed(conveyorOperatingSpeed);
            }*/
            SetConveyorSpeed(conveyorOperatingSpeed,conveyorOperatingSpeed);

            Debug.Log("ConveyorSpeed Normal");
            firstWave = false;
        }
        else
        {
            /*foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
            {
                transportableSpawner.SetConveyorsSpeed(conveyorOperatingSpeed += ConveyorSpeedIncrement);
            }*/
            conveyorOperatingSpeed += ConveyorSpeedIncrement;
            SetConveyorSpeed(conveyorOperatingSpeed,conveyorOperatingSpeed);
            
            Debug.Log("ConveyorSpeed Normal");
        }
        Debug.Log("Highest Spawning Delay");
    }

    IEnumerator EndLevel()
    {
        waitForItemsToClear(ClearItemsTimeSeconds);
        yield return null;
        soundController.PlayLevelSequenceClearedSuccessSound();
        imageList.Clean();
        FurnaceController.enabled = false;
        /*foreach (TransportableSpawner transportableSpawner in TransportableSpawners)
        {
            transportableSpawner.enabled = false;
            transportableSpawner.gameObject.SetActive(false);
        }*/

        InteriorConveyorSpawner.enabled = false;
        ExteriorConveyorSpawner.enabled = false;
        InteriorConveyorSpawner.gameObject.SetActive(false);
        ExteriorConveyorSpawner.gameObject.SetActive(false);
        
        soundController.StopAreaMusic();
    }
    
    IEnumerator StartCameraShake(float duration)
    {
        cameraMustShake = true;
        soundController.PlayLevelOneErrorSound();
        yield return new WaitForSeconds(duration);
        cameraMustShake = false;
        AreaCamera.transform.position = cameraOriginalPosition;
    }

    private void UpdateSpriteColorInList()
    {
        imageList.UpdateSpriteColor(currentListIndex, Color.black);
        currentListIndex++;
    }

    private void SetConveyorSpeed(float interiorConveyorSpeed, float exteriorConveyorSpeed)
    {
        InteriorConveyorSpawner.SetConveyorsSpeed(interiorConveyorSpeed);
        ExteriorConveyorSpawner.SetConveyorsSpeed(exteriorConveyorSpeed);
    }

    private void setItemsImageList()
    {
        itemSprites.Clear();
        FurnaceController.SequenceOfColor sequenceOfColor = FurnaceController.GetCurrentSequence();

        for (int i = 0; i < FurnaceController.GetCurrentSequenceLenght(); i++)
        {
            PickableType currentType = sequenceOfColor.types[i];
            GameObject itemImage = new GameObject();
            itemImage.AddComponent<Image>();

            switch (currentType)
            {
                case PickableType.RobotHead : itemSprites.Add(RobotHeadImage);
                    break;
                case PickableType.Crate : itemSprites.Add(CrateImage);
                    break;
                case PickableType.Gear : itemSprites.Add(GearImage);
                    break;
                case PickableType.Battery : itemSprites.Add(BatteryImage);
                    break;
                case PickableType.Pipe : itemSprites.Add(PipeImage);
                    break;
            }
        }
        imageList.CreateLayout(itemSprites,sequenceOfColor.ColorsSequence);
    }

    private void SpawnNextRequiredItem()
    {
        if (TransportableSpawners[0].canSpawnNextRequiredItem)
        {
            Debug.Log("Interior was true");
            TransportableSpawners[0].canSpawnNextRequiredItem = false;
            StartCoroutine(waitBeforeSpawningRequiredItem(1));
        }
        else if (TransportableSpawners[1].canSpawnNextRequiredItem)
        {
            Debug.Log("Exterior was true");
            TransportableSpawners[1].canSpawnNextRequiredItem = false;
            StartCoroutine(waitBeforeSpawningRequiredItem(0));
        }
    }
}