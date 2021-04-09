using System;
using System.Collections;
using System.Collections.Generic;
using Arm;
using Other;
using Photon.Pun;
using TechSupport.Informations;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class Level2Controller : MonoBehaviour, LevelController
{
    [SerializeField] private Color[] _possibleColors = null;
    [SerializeField] private Conveyor _conveyor = null;
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
    [SerializeField] private GameObject _checkPrefabs;
    [SerializeField] private Material _blurMaterial;
    [SerializeField] private Material _fontMaterial;
    [SerializeField] private Sprite _questionSprite;
    [SerializeField] private GameController.Role _emissionVisibleBy = GameController.Role.None;
    [SerializeField] private float LevelMaxAmountOfTimeSeconds = 120;
    [SerializeField] private float TimeLeftIfFirstSequenceFailed = 120;
    [SerializeField] private float TimeLeftIfSecondSequenceFailed = 80;
    [SerializeField] private float TimeLeftIfThirdSequenceFailed = 50;
    [SerializeField] private float SuccessBonusTime = 5;
    [SerializeField] private float SuccessfulSequenceBonusTime = 10;
    [SerializeField] private TimerController _timerController;
    [SerializeField] private GameObject TimerPanel;
    [SerializeField] private List<float> TimeLeftWhenWarningPlays;
    [SerializeField] private CameraEffectDisabled _cameraEffectDisabled = null;
    [SerializeField] private float _durationCameraEffectDisabled = 2.0f;
    [Tooltip("Number of times each sequence item is spawned")]
    [SerializeField] private int _requiredItemsSpawningMultiplier;

    [Tooltip("Intensity of the AreaCamera Shake Effect")]
    [SerializeField] private float cameraShakeForce = 0.3f;
    [Tooltip("Duration (Seconds) of the AreaCamera Shake effect.")]
    [SerializeField] private float cameraShakeDurationSeconds = 0.2f;
    private bool cameraMustShake = false;
    private DialogSystem _dialogSystem;
    private float _timeLeft;
    private bool _levelInProgress;
    private bool _currentSequenceFailed;
    private Coroutine timerCoroutine;
    private int _nextWarningIndex;
    
    [SerializeField] private AudioSource conveyorAudioSource = null;

    //TODO respawn items when sequence failed.
    
    public event Action<float> OnTimeChanged;
    public event Action<float> OnBonusTime;
    public event Action<float> OnWarning;
    
    
    public Color[] GetColors() => _possibleColors;
    public Color GetNextColorInSequence() => _furnace.GetNextColor();
    public int GetCurrentSequenceLenght() => _furnace.GetCurrentSequenceLenght();
    public Other.PickableType GetNextTypeInSequence() => _furnace.GetNextItemType();
    public PickableType[] GetAllNextItemTypes() => _furnace.GetAllNextItemTypes();
    public int GetIndexInCurrentSequence() => _furnace.GetIndexInCurrentSequence();
    public int GetCurrentRequiredItemIndex()
    {
        return 0;
    }
    
    public Color[] GetAllNextItemColors() => _furnace.GetAllNextItemColors();

    private int GetCurrentSequenceNumber() => _furnace.GetIndexOfCurrentSequence();

    private ImageLayout _imageList;
    private List<Sprite> _itemSprites = new List<Sprite>();
    private System.Random _random;
    private SoundController _soundController;
    private Vector3 _cameraOriginalPosition;
    private int _currentListIndex;
    private NetworkController _networkController = null;
    private float _lastTimeInverseControl = 0;
    private PhotonView _photonView = null;
    private GameController _gameController = null;

    private List<Tuple<Vector3, Quaternion, Pickable>> _spawnedPickable = new List<Tuple<Vector3, Quaternion, Pickable>>();

    private void Awake()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _random = new System.Random(_gameController.Seed);

        _soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _cameraOriginalPosition = AreaCamera.transform.position;
        _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
        _photonView = GetComponent<PhotonView>();
    }

    public void StartLevel()
    {
        _furnace.GenerateNewColorSequences(_possibleColors);

        if (_networkController.GetLocalRole() == GameController.Role.SecurityGuard)
        {
            SpawnObjects();
        }
        _soundController.PlayArea2Music();
        _techUI.GetList().Clean();

        _imageList = new GameObject().AddComponent<ImageLayout>();
        _imageList.SetBlurMaterial(_blurMaterial);
        _imageList.SetFontMaterial(_fontMaterial);
        _imageList.SetQuestion(_questionSprite);
        _imageList.SetCheckImage(_checkPrefabs);
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

        _timeLeft = LevelMaxAmountOfTimeSeconds;
        _currentSequenceFailed = false;
        //_imageList = _techUI.GetList();
        _imageList.Clean();
        setItemsImageList();
        _dialogSystem.StartDialog("Area02_start");
        _levelInProgress = true;
    }

    private void StartLevelTimer()
    {
        _dialogSystem.StartSingleLine("Area02_start_timer");
        _nextWarningIndex = 0;
        timerCoroutine = StartCoroutine(StartTimer());
        TimerPanel.SetActive(true);
        OnTimeChanged?.Invoke(_timeLeft);
    }

    public void SpawnObjects()
    {
        _photonView.RPC("CameraEffect", RpcTarget.All);

        if(_spawnedPickable.Count == 0)
        {
            List<Bounds> solutions = new List<Bounds>();
            foreach (SpawnObjectOnLineConveyor spawner in _spawners)
            {
                solutions.AddRange(spawner.GetSpawnPosition());
            }

            SpawnSequences(solutions);

            foreach (Bounds solution in solutions)
            {
                SpawnObject(solution, _possibleColors[_random.Next(0, _possibleColors.Length)]);
            }
        }
        else
        {
            foreach(Tuple<Vector3, Quaternion, Pickable> tuple in _spawnedPickable)
            {
                if(!tuple.Item3.IsGrabbed)
                {
                    tuple.Item3.SetActiveNetwork(true);
                    tuple.Item3.transform.position = tuple.Item1;
                    tuple.Item3.transform.rotation = tuple.Item2;
                    tuple.Item3.SetConsumedNetwork(false);
                }
            }
        }

    }

    [PunRPC]
    private void CameraEffect()
    {
        if(_networkController.GetLocalRole() == GameController.Role.Technician)
        {
            _cameraEffectDisabled.Enable();
            StartCoroutine(DisableCameraEffectSpawnObject(_durationCameraEffectDisabled));
        }
    }

    private IEnumerator DisableCameraEffectSpawnObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        _cameraEffectDisabled.Disable();
    }

    private void SpawnObject(Bounds solution, Color color)
    {
        GameObject randomPrefab = _transportablesPrefab[_random.Next(0, _transportablesPrefab.Length)];
        Quaternion randomRotation = Quaternion.Euler(0, _random.Next(0, 360), 0);
        GameObject gameObject = PhotonNetwork.Instantiate(randomPrefab.name, solution.center, randomRotation);
        Pickable pickable = gameObject.GetComponent<Arm.Pickable>();
        pickable.Color = color;
        pickable.Furnace = _furnace;
        pickable.SetEmissionVisibleBy(_emissionVisibleBy);
        _spawnedPickable.Add(new Tuple<Vector3, Quaternion, Pickable>(solution.center, randomRotation, pickable));
    }

    private void SpawnSequenceObject(Bounds solution, Color color, Other.PickableType type)
    {
        foreach (var t in _transportablesPrefab)
        {
            if (t.GetComponent<Arm.Pickable>().GetType() == type)
            {
                Quaternion randomRotation = Quaternion.Euler(0, _random.Next(0, 360), 0);
                GameObject gameobject = PhotonNetwork.Instantiate(t.name, solution.center, randomRotation);
                Pickable pickable = gameobject.GetComponent<Arm.Pickable>();
                pickable.Color = color;
                pickable.Furnace = _furnace;
                pickable.SetEmissionVisibleBy(_emissionVisibleBy);
                _spawnedPickable.Add(new Tuple<Vector3, Quaternion, Pickable>(solution.center, randomRotation, pickable));
                break;
            }
        }
    }

    public void FinishLevel()
    {
        _levelInProgress = false;
        TimerPanel.SetActive(false);
        StartCoroutine(DisableConveyor());
        _soundController.PlayLevelSequenceClearedSuccessSound();
        _imageList.Clean();
        _soundController.StopAreaMusic();
        _dialogSystem.StartDialog("Area02_end");
        conveyorAudioSource.Stop();
        if(_armController.IsInversedX())
        {
            _armController.InverseX();
        }
        if (_armController.IsInversedZ())
        {
            _armController.InverseZ();
        }
    }

    private IEnumerator DisableConveyor()
    {
        yield return new WaitForSeconds(5);
        _conveyor.SetSpeed(0);
    }

    public void InitiateNextSequence()
    {
        if(_furnace.SucceedSequences == 1 && !_currentSequenceFailed)
        {
            _dialogSystem.StartSingleLine("Area02_first_sequence_done");
            _armController.InverseX();
            _armController.InverseZ();
        }
        else if (_furnace.SucceedSequences ==2 && !_currentSequenceFailed)
        {
            _dialogSystem.StartSingleLine("Area02_second_sequence_done");
        }
        
        if (!_currentSequenceFailed)
        {
            _soundController.PlayLevelSequenceClearedSuccessSound();
            _timeLeft += SuccessfulSequenceBonusTime;
            OnBonusTime?.Invoke(SuccessfulSequenceBonusTime);
        }
        else
        {
            _soundController.PlayLevelOneErrorSound();
            _nextWarningIndex = 0;
            foreach (var warningTime in TimeLeftWhenWarningPlays)
            {
                if (_timeLeft > warningTime)
                {
                    break;
                }

                _nextWarningIndex++;
            }

            if (_nextWarningIndex >= TimeLeftWhenWarningPlays.Count)
            {
                _nextWarningIndex = TimeLeftWhenWarningPlays.Count - 1;
            }

        }
        _imageList.Clean();
        setItemsImageList();
        _currentListIndex = 0;
        _currentSequenceFailed = false;
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
        _furnace.WhenFurnaceConsumeRight.AddListener(OnCorrectItemDropped);
        _furnace.CheckItemOffList += UpdateSpriteColorInList;
        _furnace.OnFirstSuccessfulItemDropped += StartLevelTimer;
    }

    private void OnDisable()
    {
        _furnace.WhenFurnaceConsumedAll.RemoveListener(FinishLevel);
        _furnace.WhenFurnaceConsumeAWholeSequenceWithoutFinishing.RemoveListener(InitiateNextSequence);
        _furnace.WhenFurnaceConsumeWrong.RemoveListener(ShakeCamera);
        _furnace.WhenFurnaceConsumeRight.RemoveListener(OnCorrectItemDropped);
        _furnace.CheckItemOffList -= UpdateSpriteColorInList;
        _furnace.OnFirstSuccessfulItemDropped -= StartLevelTimer;
    }

    private void Update()
    {
        if (_levelInProgress && _furnace.SucceedSequences >= 2)
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
        _currentListIndex++;
        _imageList.SelectItem(_currentListIndex);
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
        if (solutions.Count >= _furnace.GetItemCount()*_requiredItemsSpawningMultiplier)
        {
            FurnaceController.SequenceOfColor[] sequences = _furnace.GetAllSequences();
            foreach (var sequence in sequences)
            {
                for (int i = 0; i < sequence.ColorsSequence.Length; i++)
                {
                    for (int j = 0; j < _requiredItemsSpawningMultiplier; j++)
                    {
                        int solutionIndex = _random.Next(0, solutions.Count);
                        SpawnSequenceObject(solutions[solutionIndex],sequence.ColorsSequence[i],sequence.types[i]);
                        solutions.RemoveAt(solutionIndex);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Number of items in sequences are superior to the amount of spawning positions available.");
        }
    }

    private void OnCorrectItemDropped()
    {
        if (_furnace.SucceedSequences == 1
            && _furnace.GetAllSequences()[_furnace.SucceedSequences].SucceedColors == 1)
        {
            _armController.InverseX();
            _armController.InverseZ();
        }

        _timeLeft += SuccessBonusTime;
        OnBonusTime?.Invoke(SuccessBonusTime);
        _soundController.PlayLevelPartialSequenceSuccessSound();
    }

    private void WhenTimeRunsOut()
    {
        int currentSequenceNumber = GetCurrentSequenceNumber();
        _currentSequenceFailed = true;
        _furnace.ResetCurrentSequenceSuccess();
        StopCoroutine(timerCoroutine);
        switch (currentSequenceNumber)
        {
            case 0:
                _timeLeft = TimeLeftIfFirstSequenceFailed;
                break;
            case 1:
                _timeLeft = TimeLeftIfSecondSequenceFailed;
                    break;
            case 2:
                _timeLeft = TimeLeftIfThirdSequenceFailed;
                break;
            default:
                    break;
        };
        InitiateNextSequence();
        if (_networkController.GetLocalRole() == GameController.Role.SecurityGuard)
        {
            SpawnObjects();
        }
        timerCoroutine = StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (_levelInProgress && _timeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            _timeLeft -= 1;
            OnTimeChanged?.Invoke(_timeLeft);
            

            if (_nextWarningIndex < TimeLeftWhenWarningPlays.Count)
            {
                if (_nextWarningIndex >0&&_timeLeft > TimeLeftWhenWarningPlays[_nextWarningIndex-1])
                {
                    _nextWarningIndex--;
                }

                if (_timeLeft == TimeLeftWhenWarningPlays[_nextWarningIndex])
                {
                    OnWarning?.Invoke(_timeLeft);
                    _nextWarningIndex++;
                }
            }

            if (_timeLeft == 0)
            {
                WhenTimeRunsOut();
            }
        }
    }

    public float GetTimeLeft()
    {
        return _timeLeft;
    }
}
