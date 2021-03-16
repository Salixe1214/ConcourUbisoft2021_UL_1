using System.Collections;
using System.Collections.Generic;
using Other;
using TechSupport.Informations;
using UnityEngine;
using UnityEngine.UI;

public class Level2Controller : MonoBehaviour , LevelController
{
    [SerializeField] private Color[] PossibleColors = null;
    public Color[] GetColors() => PossibleColors;
    public Color GetNextColorInSequence() => FurnaceController.GetNextColor();
    public int GetCurrentSequenceLenght() => FurnaceController.GetCurrentSequenceLenght();

    public TransportableType GetNextTypeInSequence() => FurnaceController.GetNextItemType();

    [SerializeField] private FurnaceController FurnaceController = null;
    [SerializeField] private TransportableSpawner[] TransportableSpawners = null;
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
    private ImageLayout imageList;
    private List<Sprite> itemSprites;
    private SoundController soundController;
    private int currentListIndex;
    private int spawnerIndex = 0;

    private void Awake()
    {
        itemSprites = new List<Sprite>();
    }

    public void Start()
    {
        currentListIndex = 0;
        spawnerIndex = 0;
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        FurnaceController.GenerateNewColorSequences(PossibleColors);
        FurnaceController.enabled = false;
        EnableAllSpawners(false);
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
        foreach (var spawner in TransportableSpawners)
        {
            spawner.spawnedNextNeededItem += UpdateSpawnerIndex;
        }
        FurnaceController.WhenFurnaceConsumedAll += FinishLevel;
        FurnaceController.WhenFurnaceConsumeAWholeSequenceWithoutFinishing += InitiateNextSequence;
        FurnaceController.WhenFurnaceConsumeWrong += ShakeCamera;
        FurnaceController.CheckItemOffList += UpdateSpriteColorInList;
    }

    private void OnDisable()
    {
        foreach (var spawner in TransportableSpawners)
        {
            spawner.spawnedNextNeededItem -= UpdateSpawnerIndex;
        }
        FurnaceController.WhenFurnaceConsumedAll -= FinishLevel;
        FurnaceController.WhenFurnaceConsumeAWholeSequenceWithoutFinishing -= InitiateNextSequence;
        FurnaceController.WhenFurnaceConsumeWrong -= ShakeCamera;
        FurnaceController.CheckItemOffList -= UpdateSpriteColorInList;
    }

    public void FinishLevel()
    {
        SetAllConveyorsSpeed(MaxConveyorSpeed);
        StartCoroutine(EndLevel());
    }

    public void StartLevel()
    {
        Debug.Log("StartLevel");
        imageList = techUI.GetList();
        firstWave = true;
        FurnaceController.enabled = true;
        EnableAllSpawners(true);
        ActivateItemSpawning(false);
        SetAllConveyorsSpeed(MaxConveyorSpeed);
        Debug.Log("ConveyorSpeed Max");
        TransportableSpawners[spawnerIndex].CanSpawnNextNeededItem = true;
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
        soundController.PlayArea2Music();
    }

    public void InitiateNextSequence()
    {
        ActivateItemSpawning(false);
        soundController.PlayLevelSequenceClearedSuccessSound();
        SetAllConveyorsSpeed(MaxConveyorSpeed);
        StartCoroutine(SpawnFreshItems(FastItemSpawningTimeSeconds));
    }

    public void ShakeCamera()
    {
        StartCoroutine(StartCameraShake(cameraShakeDurationSeconds));
    }

    private void ActivateItemSpawning(bool canSpawn)
    {
        ActivateSpawningForAllSpawners(canSpawn);
    }

    private void SetDelayBetweenItemSpawns(Vector2 delayRange)
    {
        SetSpawningDelayForAllSpawners(delayRange);
    }

    IEnumerator waitForItemsToClear(float seconds)
    {
        yield return new WaitForSeconds(seconds);
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
        SetDelayBetweenItemSpawns(DelayBetweenItemSpawnsSecondsLowest);
        Debug.Log("Lowest Spawning Delay");
        
        setItemsImageList();
        ActivateItemSpawning(true);
        yield return new WaitForSeconds(seconds);
        SetDelayBetweenItemSpawns(DelayBetweenItemSpawnsSecondsHighest); 
        if (firstWave)
        {
            SetAllConveyorsSpeed(conveyorOperatingSpeed);
            Debug.Log("ConveyorSpeed Normal");
            firstWave = false;
        }
        else
        {            
            SetAllConveyorsSpeed(conveyorOperatingSpeed+=ConveyorSpeedIncrement); 
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
        EnableAllSpawners(false);
        ActivateItemSpawning(false);
        soundController.StopArea2Music();
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

    private void HighLightCurrentItemInList()
    {
        
    }

    private void UpdateSpawnerIndex()
    {
        TransportableSpawners[spawnerIndex].CanSpawnNextNeededItem = false;
        spawnerIndex++;
        if (spawnerIndex == TransportableSpawners.Length)
        {
            spawnerIndex = 0;
        }
        TransportableSpawners[spawnerIndex].CanSpawnNextNeededItem = true;
    }

    private void SetAllConveyorsSpeed(float speed)
    {
        foreach (var spawner in TransportableSpawners)
        {
            spawner.SetConveyorsSpeed(speed);
        }
    }

    private void EnableAllSpawners(bool enabled)
    {
        foreach (var spawner in TransportableSpawners)
        {
            spawner.enabled = enabled;
        }
    }

    private void ActivateSpawningForAllSpawners(bool spawningActivated)
    {
        foreach (var spawner in TransportableSpawners)
        {
            spawner.ActivateSpawning(spawningActivated);
        }
    }

    private void SetSpawningDelayForAllSpawners(Vector2 delayRange)
    {
        foreach (var spawner in TransportableSpawners)
        {
            spawner.SetDelayBetweenSpawns(delayRange);
        }
    }

    private void setItemsImageList()
    {
        itemSprites.Clear();
        FurnaceController.SequenceOfColor sequenceOfColor = FurnaceController.GetCurrentSequence();

        for (int i = 0; i < FurnaceController.GetCurrentSequenceLenght(); i++)
        {
            TransportableType currentType = sequenceOfColor.types[i];
            GameObject itemImage = new GameObject();
            itemImage.AddComponent<Image>();

            switch (currentType)
            {
                case TransportableType.RobotHead : itemSprites.Add(RobotHeadImage);
                    break;
                case TransportableType.Crate : itemSprites.Add(CrateImage);
                    break;
                case TransportableType.Gear : itemSprites.Add(GearImage);
                    break;
                case TransportableType.Battery : itemSprites.Add(BatteryImage);
                    break;
                case TransportableType.Pipe : itemSprites.Add(PipeImage);
                    break;
            }
        }
        imageList.CreateLayout(itemSprites,sequenceOfColor.ColorsSequence);
    }
    
   /* public void SpawnObjects()
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
    }*/
}
