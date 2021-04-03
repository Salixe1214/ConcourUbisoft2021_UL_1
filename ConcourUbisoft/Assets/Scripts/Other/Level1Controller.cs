using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using JetBrains.Annotations;
using Other;
using TechSupport.Informations;
using Unity.Collections;
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
    public PickableType[] GetAllNextItemTypes() => FurnaceController.GetAllNextItemTypes();
    public Color[] GetAllNextItemColors() => FurnaceController.GetAllNextItemColors();
    public int GetIndexInCurrentSequence() => FurnaceController.GetIndexInCurrentSequence();
    public int GetCurrentRequiredItemIndex() => GetCurrentRequiredSpawningIndex();
    

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
    [SerializeField] private float delayBeforeNextRequiredItem = 3f;
    

    private float conveyorOperatingSpeed;
    private bool firstWave = false;
    private Vector3 cameraOriginalPosition;
    private bool cameraMustShake = false;
    private ImageLayout imageList;
    private List<Sprite> itemSprites;
    private SoundController soundController;
    private int currentListIndex;
    private List<TransportableSpawner> TransportableSpawners;
    private int currentRequiredItemIndex = 0;
    private DialogSystem _dialogSystem;
    private float actualDelayNextRequiredItem;
    
    public event Action<float> OnTimeChanged;
    public event Action<float> OnBonusTime;

    public event Action<float> OnWarning;

    private void Awake()
    {
        itemSprites = new List<Sprite>();
        TransportableSpawners = new List<TransportableSpawner>();
    }

    public void Start()
    {
        _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
        TransportableSpawners.Add(InteriorConveyorSpawner);
        TransportableSpawners.Add(ExteriorConveyorSpawner);
        currentListIndex = 0;
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        FurnaceController.GenerateNewColorSequences(PossibleColors);
        FurnaceController.enabled = false;
        conveyorOperatingSpeed = MinConveyorSpeed;
        cameraOriginalPosition = AreaCamera.transform.position;
        actualDelayNextRequiredItem = delayBeforeNextRequiredItem;
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
        FurnaceController.WhenFurnaceConsumeRight.AddListener(OnCorrectItemDropped);
        InteriorConveyorSpawner.requiredItemHasSpawned += SpawnNextRequiredItem;
        ExteriorConveyorSpawner.requiredItemHasSpawned += SpawnNextRequiredItem;
    }

    private void OnDisable()
    {
        FurnaceController.WhenFurnaceConsumedAll.RemoveListener(FinishLevel);
        FurnaceController.WhenFurnaceConsumeAWholeSequenceWithoutFinishing.RemoveListener(InitiateNextSequence);
        FurnaceController.WhenFurnaceConsumeWrong.RemoveListener(ShakeCamera);
        FurnaceController.CheckItemOffList -= UpdateSpriteColorInList;
        FurnaceController.WhenFurnaceConsumeRight.RemoveListener(OnCorrectItemDropped);
        InteriorConveyorSpawner.requiredItemHasSpawned -= SpawnNextRequiredItem;
        ExteriorConveyorSpawner.requiredItemHasSpawned -= SpawnNextRequiredItem;
    }

    public void FinishLevel()
    {
        SetConveyorSpeed(MaxInteriorConveyorSpeed,MaxExteriorConveyorSpeed);
        ActivateItemSpawning(false);
        Debug.Log("Items are not spawning");
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
        Debug.Log(TransportableSpawners[0]);
        Debug.Log(TransportableSpawners[1]);
        ActivateItemSpawning(false);
        Debug.Log("Items are not spawning");
        SetConveyorSpeed(MaxInteriorConveyorSpeed,MaxExteriorConveyorSpeed);
        Debug.Log("ConveyorSpeed Max");
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
        StartCoroutine(StartLevelDialog(2));
    }

    private void InitiateNextSequence()
    {
        actualDelayNextRequiredItem = delayBeforeNextRequiredItem;
        ActivateItemSpawning(false);
        Debug.Log("Items are not spawning");
        soundController.PlayLevelSequenceClearedSuccessSound();
        SetConveyorSpeed(MaxInteriorConveyorSpeed,MaxExteriorConveyorSpeed);
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
    }

    private void ShakeCamera()
    {
        StartCoroutine(StartCameraShake(cameraShakeDurationSeconds));
    }

    private void ActivateItemSpawning(bool canSpawn)
    {
        if (canSpawn)
        {
            ExteriorConveyorSpawner.canSpawnNextRequiredItem = true;
            currentRequiredItemIndex = 0;
            Debug.Log("Spawner Values Set");
            Debug.Log(InteriorConveyorSpawner.canSpawnNextRequiredItem);
            Debug.Log(ExteriorConveyorSpawner.canSpawnNextRequiredItem);
        }
        else
        {
            InteriorConveyorSpawner.canSpawnNextRequiredItem = false;
            ExteriorConveyorSpawner.canSpawnNextRequiredItem = false;
            Debug.Log("Spawner Values Set");
            Debug.Log(InteriorConveyorSpawner.canSpawnNextRequiredItem);
            Debug.Log(ExteriorConveyorSpawner.canSpawnNextRequiredItem);
        }
        InteriorConveyorSpawner.ActivateSpawning(canSpawn);
        ExteriorConveyorSpawner.ActivateSpawning(canSpawn);
        Debug.Log("Can spawn changed");
        Debug.Log(canSpawn);
    }

    private void SetDelayBetweenItemSpawns(Vector2 delayRangeInteriorConveyor, Vector2 delayRangeExteriorConveyor)
    {
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
        TransportableSpawners[nextSpawnerIndex].canSpawnNextRequiredItem = true;
    }

    IEnumerator SpawnFreshItems(float seconds)
    {
        currentListIndex = 0;
        if (!firstWave)
        {
            imageList.Clean();
            yield return waitForItemsToClear(ClearItemsTimeSeconds);
        }      
        SetDelayBetweenItemSpawns(DelayItemSpawnsLowestInteriorConveyor,DelayItemSpawnsLowestExteriorConveyor);
        
        setItemsImageList();
        ActivateItemSpawning(true);
        yield return new WaitForSeconds(seconds);
        SetDelayBetweenItemSpawns(DelayItemSpawnsHighestInteriorConveyor,DelayItemSpawnsHighestExteriorConveyor);
        if (firstWave)
        {
            SetConveyorSpeed(conveyorOperatingSpeed,conveyorOperatingSpeed);

            firstWave = false;
        }
        else
        {
            conveyorOperatingSpeed += ConveyorSpeedIncrement;
            SetConveyorSpeed(conveyorOperatingSpeed,conveyorOperatingSpeed);
            
        }
    }

    IEnumerator EndLevel()
    {
        imageList.Clean();
        StartCoroutine(waitForItemsToClear(ClearItemsTimeSeconds));
        soundController.PlayLevelSequenceClearedSuccessSound();
        FurnaceController.enabled = false;
        soundController.StopAreaMusic();
        InteriorConveyorSpawner.gameObject.SetActive(false);
        ExteriorConveyorSpawner.gameObject.SetActive(false);
        yield return null;
        _dialogSystem.StartDialog("Area01_end");
    }
    
    IEnumerator StartCameraShake(float duration)
    {
        cameraMustShake = true;
        soundController.PlayLevelOneErrorSound();
        yield return new WaitForSeconds(duration);
        cameraMustShake = false;
        AreaCamera.transform.position = cameraOriginalPosition;
    }
    
    IEnumerator StartLevelDialog(float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        _dialogSystem.StartDialog("Area01_start");
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
            Debug.Log("Interior was true and spawned");
            TransportableSpawners[0].canSpawnNextRequiredItem = false;
            StartCoroutine(waitBeforeSpawningRequiredItem(1));
        }
        else if (TransportableSpawners[1].canSpawnNextRequiredItem)
        {
            Debug.Log("Exterior was true and spawned");
            TransportableSpawners[1].canSpawnNextRequiredItem = false;
            StartCoroutine(waitBeforeSpawningRequiredItem(0));
        }
    }

    private int GetCurrentRequiredSpawningIndex()
    {
        if (currentRequiredItemIndex >= GetCurrentSequenceLenght())
        {
            currentRequiredItemIndex = GetIndexInCurrentSequence();
        }
        currentRequiredItemIndex++;
        return currentRequiredItemIndex-1;
    }

    private void OnCorrectItemDropped()
    {
        soundController.PlayLevelPartialSequenceSuccessSound();
        actualDelayNextRequiredItem += 1;
    }
}