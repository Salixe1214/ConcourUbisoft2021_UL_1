using System.Collections;
using System.Collections.Generic;
using Other;
using TechSupport.Informations;
using UnityEngine;
using UnityEngine.UI;

public class Level2Controller : MonoBehaviour, LevelController
{
    [SerializeField] private Color[] _possibleColors = null;
    [SerializeField] private GameObject[] _transportablesPrefab = null;
    [SerializeField] private SpawnObjectOnLineConveyor[] _spawners = null;
    [SerializeField] private FurnaceController _furnace = null;
    [SerializeField] private InformationsSystem _techUI;
    [SerializeField] private Sprite RobotHeadImage;
    [SerializeField] private Sprite CrateImage;
    [SerializeField] private Sprite GearImage;
    [SerializeField] private Sprite BatteryImage;
    [SerializeField] private Sprite PipeImage;
    [SerializeField] private Camera AreaCamera = null;

    [Tooltip("Intensity of the AreaCamera Shake Effect")]
    [SerializeField] private float cameraShakeForce = 0.3f;
    [Tooltip("Duration (Seconds) of the AreaCamera Shake effect.")]
    [SerializeField] private float cameraShakeDurationSeconds = 0.2f;
    private bool cameraMustShake = false;

    public Color[] GetColors() => _possibleColors;
    public Color GetNextColorInSequence() => _furnace.GetNextColor();
    public int GetCurrentSequenceLenght() => _furnace.GetCurrentSequenceLenght();
    public TransportableType GetNextTypeInSequence() => _furnace.GetNextItemType();

    private ImageLayout _imageList;
    private List<Sprite> _itemSprites = new List<Sprite>();
    private System.Random _random = new System.Random(0);
    private SoundController _soundController;
    private Vector3 _cameraOriginalPosition;
    private int _currentListIndex;

    private void Awake()
    {
        _soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _cameraOriginalPosition = AreaCamera.transform.position;
    }

    public void StartLevel()
    {
        _furnace.GenerateNewColorSequences(_possibleColors);
        SpawnObjects();

        _imageList = _techUI.GetList();
        setItemsImageList();
    }

    public void SpawnObjects()
    {
        List<Bounds> solutions = new List<Bounds>();
        foreach(SpawnObjectOnLineConveyor spawner in _spawners)
        {
            solutions.AddRange(spawner.GetSpawnPosition());
        }
        
        SpawnSequences(solutions);

        foreach (Bounds solution in solutions)
        {
            SpawnObject(solution, _possibleColors[_random.Next(0, _possibleColors.Length)]);
        }
    }

    private void SpawnObject(Bounds solution, Color color)
    {
        GameObject randomPrefab = _transportablesPrefab[_random.Next(0, _transportablesPrefab.Length)];
        GameObject transportable = Instantiate(randomPrefab, solution.center, Quaternion.identity);
        transportable.gameObject.GetComponent<TransportableByConveyor>().Color = color; 
    }

    private void SpawnSequenceObject(Bounds solution, Color color, TransportableType type)
    {
        foreach (var t in _transportablesPrefab)
        {
            if (t.GetComponent<TransportableByConveyor>().GetType() == type)
            {
                GameObject transportable = Instantiate(t, solution.center, Quaternion.identity);
                transportable.gameObject.GetComponent<TransportableByConveyor>().Color = color;
                break;
            }
        }
    }

    public void FinishLevel()
    {
        _soundController.PlayLevelSequenceClearedSuccessSound();
        _imageList.Clean();
    }

    public void InitiateNextSequence()
    {
        _soundController.PlayLevelSequenceClearedSuccessSound();
        _imageList.Clean();
        setItemsImageList();
    }

    public void ShakeCamera()
    {
        StartCoroutine(StartCameraShake(cameraShakeDurationSeconds));
    }

    IEnumerator StartCameraShake(float duration)
    {
        cameraMustShake = true;
        _soundController.PlayLevelOneErrorSound();
        yield return new WaitForSeconds(duration);
        cameraMustShake = false;
        AreaCamera.transform.position = _cameraOriginalPosition;
    }

    private void OnEnable()
    {
        _furnace.WhenFurnaceConsumedAll += FinishLevel;
        _furnace.WhenFurnaceConsumeAWholeSequenceWithoutFinishing += InitiateNextSequence;
        _furnace.WhenFurnaceConsumeWrong += ShakeCamera;
        _furnace.CheckItemOffList += UpdateSpriteColorInList;
    }

    private void OnDisable()
    {
        _furnace.WhenFurnaceConsumedAll -= FinishLevel;
        _furnace.WhenFurnaceConsumeAWholeSequenceWithoutFinishing -= InitiateNextSequence;
        _furnace.WhenFurnaceConsumeWrong -= ShakeCamera;
        _furnace.CheckItemOffList -= UpdateSpriteColorInList;
    }

    private void Update()
    {
        if (cameraMustShake)
        {
            AreaCamera.transform.position = _cameraOriginalPosition + Random.insideUnitSphere * cameraShakeForce;
        }
    }

    private void UpdateSpriteColorInList()
    {
        _imageList.UpdateSpriteColor(_currentListIndex, Color.black);
        _currentListIndex++;
    }

    private void setItemsImageList()
    {
        _itemSprites.Clear();
        FurnaceController.SequenceOfColor sequenceOfColor = _furnace.GetCurrentSequence();

        for (int i = 0; i < _furnace.GetCurrentSequenceLenght(); i++)
        {
            TransportableType currentType = sequenceOfColor.types[i];
            GameObject itemImage = new GameObject();
            itemImage.AddComponent<Image>();

            switch (currentType)
            {
                case TransportableType.RobotHead:
                    _itemSprites.Add(RobotHeadImage);
                    break;
                case TransportableType.Crate:
                    _itemSprites.Add(CrateImage);
                    break;
                case TransportableType.Gear:
                    _itemSprites.Add(GearImage);
                    break;
                case TransportableType.Battery:
                    _itemSprites.Add(BatteryImage);
                    break;
                case TransportableType.Pipe:
                    _itemSprites.Add(PipeImage);
                    break;
            }
        }
        _imageList.CreateLayout(_itemSprites, sequenceOfColor.ColorsSequence);
    }

    private void SpawnSequences(List<Bounds> solutions)
    {
        if (solutions.Count >= _furnace.GetItemCount())
        {
            FurnaceController.SequenceOfColor[] sequences = _furnace.GetAllSequences();
            foreach (var sequence in sequences)
            {
                for (int i = 0; i < sequence.ColorsSequence.Length; i++)
                {
                    int solutionIndex = _random.Next(0, solutions.Count);
                    SpawnSequenceObject(solutions[solutionIndex],sequence.ColorsSequence[i],sequence.types[i]);
                    solutions.RemoveAt(solutionIndex);
                }
            }
        }

        else
        {
            Debug.Log("Number of items in sequences are superior to the amount of spawning positions available.");
        }

        
    }
}
