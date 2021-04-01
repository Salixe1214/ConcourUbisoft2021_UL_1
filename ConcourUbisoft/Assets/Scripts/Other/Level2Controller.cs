using System.Collections;
using System.Collections.Generic;
using Arm;
using Other;
using Photon.Pun;
using TechSupport.Informations;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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
    [SerializeField] private ArmController _armController = null;
    [SerializeField] private float _delayInverse = 10;
    [SerializeField] private Canvas _information = null;
    [SerializeField] private Font _font;
    [SerializeField] private GameController.Role _emissionVisibleBy = GameController.Role.None;

    [Tooltip("Intensity of the AreaCamera Shake Effect")]
    [SerializeField] private float cameraShakeForce = 0.3f;
    [Tooltip("Duration (Seconds) of the AreaCamera Shake effect.")]
    [SerializeField] private float cameraShakeDurationSeconds = 0.2f;
    private bool cameraMustShake = false;
    private DialogSystem _dialogSystem;

    public Color[] GetColors() => _possibleColors;
    public Color GetNextColorInSequence() => _furnace.GetNextColor();
    public int GetCurrentSequenceLenght() => _furnace.GetCurrentSequenceLenght();
    public Other.PickableType GetNextTypeInSequence() => _furnace.GetNextItemType();
    public PickableType[] GetAllNextItemTypes() => _furnace.GetAllNextItemTypes();
    public int GetCurrentSequenceIndex() => _furnace.GetCurrentSequenceIndex();
    public int GetCurrentRequiredItemIndex()
    {
        return 0;
    }
    public Color[] GetAllNextItemColors() => _furnace.GetAllNextItemColors();
    

    private ImageLayout _imageList;
    private List<Sprite> _itemSprites = new List<Sprite>();
    private System.Random _random = new System.Random(0);
    private SoundController _soundController;
    private Vector3 _cameraOriginalPosition;
    private int _currentListIndex;
    private NetworkController _networkController = null;
    private float _lastTimeInverseControl = 0;


    private void Awake()
    {
        _soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _cameraOriginalPosition = AreaCamera.transform.position;
        _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();

    }

    public void StartLevel()
    {
        _soundController.PlayArea2Music();
        _furnace.GenerateNewColorSequences(_possibleColors);

        if(_networkController.GetLocalRole() == GameController.Role.SecurityGuard)
        {
            SpawnObjects();
        }

        _techUI.GetList().Clean();

        _imageList = new GameObject().AddComponent<ImageLayout>();
        _imageList.GetComponent<RectTransform>().SetParent(_information.transform);
        _imageList.Font = _font;
        _imageList.TextOffset = -75.0f;
        RectTransform rectTransform = _imageList.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(430, 175);
        rectTransform.anchorMax = new Vector2(0,0);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.pivot = new Vector2(0,0);
        rectTransform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        rectTransform.localPosition = new Vector3(0,0,0);
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.anchoredPosition = new Vector2(0,0);

        //_imageList = _techUI.GetList();
        _imageList.Clean();
        setItemsImageList();
        _dialogSystem.StartDialog("Area02_start");
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

        GameObject gameobject = PhotonNetwork.Instantiate(randomPrefab.name, solution.center, Quaternion.identity);
        Pickable pickable = gameobject.GetComponent<Arm.Pickable>();
        pickable.Color = color;
        pickable.Furnace = _furnace;
        pickable.SetEmissionVisibleBy(_emissionVisibleBy);
    }

    private void SpawnSequenceObject(Bounds solution, Color color, Other.PickableType type)
    {
        foreach (var t in _transportablesPrefab)
        {
            if (t.GetComponent<Arm.Pickable>().GetType() == type)
            {
                GameObject gameobject = PhotonNetwork.Instantiate(t.name, solution.center, Quaternion.identity);
                Pickable pickable = gameobject.GetComponent<Arm.Pickable>();
                pickable.Color = color;
                pickable.Furnace = _furnace;
                pickable.SetEmissionVisibleBy(_emissionVisibleBy);
                break;
            }
        }
    }

    public void FinishLevel()
    {
        _soundController.PlayLevelSequenceClearedSuccessSound();
        _imageList.Clean();
        _soundController.StopAreaMusic();
        _dialogSystem.StartDialog("Area02_end");
    }

    public void InitiateNextSequence()
    {
        if(_furnace.SucceedSequences == 1)
        {
            _dialogSystem.StartDialog("Area02_first_sequence_done");
            _armController.InverseX();
            _armController.InverseZ();
        }
        else if (_furnace.SucceedSequences ==2)
        {
            _dialogSystem.StartDialog("Area02_second_sequence_done");
        }
        _soundController.PlayLevelSequenceClearedSuccessSound();
        _imageList.Clean();
        setItemsImageList();
        _currentListIndex = 0;
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
        _furnace.WhenFurnaceConsumedAll.AddListener(FinishLevel);
        _furnace.WhenFurnaceConsumeAWholeSequenceWithoutFinishing.AddListener(InitiateNextSequence);
        _furnace.WhenFurnaceConsumeWrong.AddListener(ShakeCamera);
        _furnace.CheckItemOffList += UpdateSpriteColorInList;
    }

    private void OnDisable()
    {
        _furnace.WhenFurnaceConsumedAll.RemoveListener(FinishLevel);
        _furnace.WhenFurnaceConsumeAWholeSequenceWithoutFinishing.RemoveListener(InitiateNextSequence);
        _furnace.WhenFurnaceConsumeWrong.RemoveListener(ShakeCamera);
        _furnace.CheckItemOffList -= UpdateSpriteColorInList;
    }

    private void Update()
    {
        if (_furnace.SucceedSequences >= 2)
        {
            if(Time.time - _lastTimeInverseControl > _delayInverse)
            {
                _armController.InverseX();
                _armController.InverseZ();
                _lastTimeInverseControl = Time.time;
            }
        }

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
            Other.PickableType currentType = sequenceOfColor.types[i];
            GameObject itemImage = new GameObject();
            itemImage.AddComponent<Image>();

            switch (currentType)
            {
                case Other.PickableType.RobotHead:
                    _itemSprites.Add(RobotHeadImage);
                    break;
                case Other.PickableType.Crate:
                    _itemSprites.Add(CrateImage);
                    break;
                case Other.PickableType.Gear:
                    _itemSprites.Add(GearImage);
                    break;
                case Other.PickableType.Battery:
                    _itemSprites.Add(BatteryImage);
                    break;
                case Other.PickableType.Pipe:
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
