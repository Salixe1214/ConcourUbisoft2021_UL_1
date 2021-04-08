using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Inputs;
using Photon.Voice;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    // UI components
    [SerializeField] private Image leftCharSlot; //< Character slot at the left
    [SerializeField] private Image rightCharSlot; //< Character slot at the right
    [SerializeField] private Text textSlot; //< Text slot
    [SerializeField] private GameObject panel = null;
    [SerializeField] private float _delayBetweenCharReveal = 0.1f;
    [SerializeField] private Sprite[] originalKeys;
    [SerializeField] private Image skipKey;
    private Sprite altKey;
    private Sprite _altSkipKey;
    private Sprite _originalSkipKey;
    private GameController _gameController;

    // Sprites
    [SerializeField] private Sprite char1Sprite;
    [SerializeField] private Sprite char2Sprite;
    [SerializeField] private Sprite char3Sprite;
    [SerializeField] private Sprite char4Sprite;

    // Line separators
    [SerializeField] private char lineSep = '\n';
    [SerializeField] private char itemSep = ';';

    private List<string>
        _lines = new List<string>(); //< Contains all the lines (and the informations of the characters shown)
    /*
     * Script text format:
     * -------------------
     *
     * [id of the left character],[id of the right character],[line]
     *
     * ex:
     * 1;0;Hi, my name is john!
     *
     * IDS:
     * 1 ,2 or 3 for character 1 , 2  or 3(defined by the sprites)
     * 0 for none
     */

    private bool isEmpty = true;
    private bool _isReading = false;

    private NetworkController _networkController = null;
    [SerializeField] private AudioSource _audioSource;

    // Long press parameters
    [SerializeField] private float longPressDuration = 1;
    private bool bIsPressed = false;
    private float bDownTime = 0;

    private IEnumerator _slowReadCoroutine;
    private Controller _actualController;
    private InputManager _inputManager;

    private bool _skipAfter = false;
    [SerializeField] private float skipTime = 5;

    private float rotationLeft = 0;
    private float rotationRight = 180;

    [SerializeField] private string normalColor = "black";
    [SerializeField] private string altColor = "blue";

    [SerializeField] private AudioSource endMusic;
    [SerializeField] private AudioClip endClip;
    private float[] _endTime =  {0f, 2.179f, 4.278f, 6.449f, 8.658f, 10.857f, 13.055f, 23.567f };
    private bool isEnd = false;

    public event Action OnFinalDialog;
    public event Action OnFinalDialogMusicStart;

    private void Awake()
    {
        //_originalSkipKey = originalKeys[0];
        //_altSkipKey = altKey;
        _inputManager = GameObject.FindWithTag("InputManager")?.GetComponent<InputManager>();
        _gameController = GameObject.FindWithTag("GameController")?.GetComponent<GameController>();
        OnControllerTypeChanged();
        rightCharSlot.transform.localRotation = Quaternion.Euler(0,180,0);
        leftCharSlot.enabled = false;
        rightCharSlot.enabled = false;
        textSlot.enabled = false;
        // textSlot.resizeTextForBestFit = true; //< Have a nice effect but math doesn't like it. Yeah, I dont -Mat

        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();

        if(_networkController.GetLocalRole() == GameController.Role.SecurityGuard)
        {
            var temp = leftCharSlot;
            leftCharSlot = rightCharSlot;
            rightCharSlot = temp;

            var tempRotation = rotationLeft;
            rotationLeft = rotationRight;
            rotationRight = tempRotation;
        }
    }

    void Update()
    {
        if (!_gameController.IsGameMenuOpen && !_gameController.IsEndGameMenuOpen&&(Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("ConfirmDialogPS")||Input.GetButtonDown("ConfirmDialogXBO")) && !isEnd)
        {
            bDownTime = 0;
            bIsPressed = true;
        }

        if (bIsPressed && (Input.GetKey(KeyCode.Q) || Input.GetButton("ConfirmDialogPS")||Input.GetButton("ConfirmDialogXBO")))
        {
            bDownTime += Time.deltaTime;

            skipKey.sprite = _altSkipKey;
            
            skipKey.color = Color.Lerp(Color.black, Color.white, bDownTime);

            if (bDownTime >= longPressDuration)
            {
                skipKey.sprite = _originalSkipKey;
                skipKey.color = Color.white;
                bIsPressed = false;
                StartCoroutine(SkipAll());
            }
        }

        if (bIsPressed && (Input.GetKeyUp(KeyCode.Q) || Input.GetButtonUp("ConfirmDialogPS")||Input.GetButtonUp("ConfirmDialogXBO")))
        {
            bIsPressed = false;
            
            skipKey.sprite = _originalSkipKey;
            skipKey.color = Color.white;

            if(!_isReading)
            {
                ReadLine();
            }
            else
            {
                _isReading = false;
            }
        }
    }

    private void OnEnable()
    {
        _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
    }

    private void OnDisable()
    {
        _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
    }

    public void ReadLine(bool pSilence = false)
    {
        if (_lines.Count > 0)
        {
            string currentLine = _lines[0]; //< Extracting the first line of the list
            string[] parsedLine = currentLine.Split(itemSep);
            _lines.Remove(currentLine);
            
            int leftCharacterID, rightCharacterID; //< IDs of the left and right character (according to sprites)
            int.TryParse(parsedLine[0], out leftCharacterID); //< Extracting ID of the left character
            int.TryParse(parsedLine[1], out rightCharacterID);//< Extracting ID of the right character
        
            // Left slot
            switch (leftCharacterID)
            {
                case 0:
                    leftCharSlot.transform.localRotation = Quaternion.Euler(0, rotationLeft, 0);
                    leftCharSlot.sprite = null;
                    leftCharSlot.color = Color.clear;
                    leftCharSlot.transform.parent.gameObject.SetActive(false);
                    break;
                case 1:
                    leftCharSlot.transform.localRotation = Quaternion.Euler(0, rotationLeft, 0);
                    leftCharSlot.sprite = char1Sprite;
                    leftCharSlot.color = Color.white;
                    leftCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 2:
                    leftCharSlot.transform.localRotation = Quaternion.Euler(0, rotationLeft, 0);
                    leftCharSlot.sprite = char2Sprite;
                    leftCharSlot.color = Color.white;
                    leftCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 3:
                    leftCharSlot.transform.localRotation = Quaternion.Euler(0, rotationLeft, 0);
                    leftCharSlot.sprite = char3Sprite;
                    leftCharSlot.color = Color.white;
                    leftCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 4:
                    leftCharSlot.transform.localRotation = Quaternion.Euler(0, rotationLeft, 0);
                    leftCharSlot.sprite = char4Sprite;
                    leftCharSlot.color = Color.white;
                    leftCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                default:
                    leftCharSlot.sprite = null;
                    leftCharSlot.color = Color.clear;
                    leftCharSlot.transform.parent.gameObject.SetActive(false);
                    break;
            }
            
            // Right slot
            switch (rightCharacterID)
            {
                case 0:
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0,rotationRight,0);
                    rightCharSlot.sprite = null;
                    rightCharSlot.color = Color.clear;
                    rightCharSlot.transform.parent.gameObject.SetActive(false);
                    break;
                case 1:
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0, rotationRight, 0);
                    rightCharSlot.sprite = char1Sprite;
                    rightCharSlot.color = Color.white;
                    rightCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 2:
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0, rotationRight, 0);
                    rightCharSlot.sprite = char2Sprite;
                    rightCharSlot.color = Color.white;
                    rightCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 3:
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0, rotationRight, 0);
                    rightCharSlot.sprite = char3Sprite;
                    rightCharSlot.color = Color.white;
                    rightCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 4:
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0, rotationRight, 0);
                    rightCharSlot.sprite = char4Sprite;
                    rightCharSlot.color = Color.white;
                    rightCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                default:
                    rightCharSlot.sprite = null;
                    rightCharSlot.color = Color.clear;
                    rightCharSlot.transform.parent.gameObject.SetActive(false);
                    break;
            }

            _slowReadCoroutine = SlowRead(parsedLine[2], pSilence, _skipAfter);
            StartCoroutine(_slowReadCoroutine);
        }
        else
        {
            leftCharSlot.color = Color.clear;
            leftCharSlot.enabled = false;
            leftCharSlot.transform.parent.gameObject.SetActive(false);

            rightCharSlot.color = Color.clear;
            rightCharSlot.enabled = false;
            rightCharSlot.transform.parent.gameObject.SetActive(false);


            textSlot.text = "";
            textSlot.enabled = false;
            panel.SetActive(false);

            isEmpty = true;
        }
    }
    
    public void StartDialog(string pFile, string pAltColor = "blue", string pNormalColor = "black", bool pSkipAfter = false)
    {
        _skipAfter = pSkipAfter;

        altColor = pAltColor;
        normalColor = pNormalColor;
        
        // Loading text file
        TextAsset txtAsset = Resources.Load("Dialog/" + pFile) as TextAsset;
        string rawTxt = "";
        if(txtAsset != null)
            rawTxt = txtAsset.ToString();
        
        // Formating the lines in a list
        string[] tmpLines = rawTxt.Split(lineSep);
        
        // Adding each line to _lines (except the last, which is empty
        for(int i = 0 ; i < tmpLines.Length ; i++)
            if (tmpLines[i].Length > 4)
            {
                _lines.Add(tmpLines[i]);
            }
        
        if (isEmpty)
        {
            isEmpty = false;
            
            leftCharSlot.enabled = true;
            rightCharSlot.enabled = true;
            textSlot.enabled = true;
            skipKey.enabled = true;
            panel.SetActive(true);

            leftCharSlot.color = Color.clear;
            rightCharSlot.color = Color.clear;
            textSlot.text = "";
            
            if(!_isReading)
            {
                ReadLine();
            }
            else
            {
                _isReading = false;
            }
            
            /*if(pAutoRead)
                StartCoroutine(ReadAll(autoReadDelay));*/
        }
    }
    
    IEnumerator SlowRead(string text, bool pSilence, bool pSkipAfter = false)
    {
        _isReading = true;
        textSlot.text = "";
        
        string newText = "";
        bool bold = false;
        string color = normalColor;
        int i = 0;
        
        while (i < text.Length)
        {
            switch (text.ToCharArray()[i])
            {
                case '%':
                    if (color == normalColor)
                        color = altColor;
                    else if (color == altColor)
                        color = normalColor;
                    break;
                case '$':
                    bold = !bold;
                    break;
                default:
                    if (bold)
                    {
                        newText += "<b><color="+color+">"+text.ToCharArray()[i] + "</color></b>";
                        textSlot.text += "<b><color="+color+">"+text.ToCharArray()[i] + "</color></b>";
                    }
                    else
                    {
                        newText += "<color="+color+">"+text.ToCharArray()[i] + "</color>";
                        textSlot.text += "<color="+color+">"+text.ToCharArray()[i] + "</color>";
                    }
                    break;
            }
            
            
            if (_audioSource.isPlaying == false && !pSilence)
            {
                _audioSource.Play();
            }
            
            i++;
            if(_isReading)
                yield return new WaitForSeconds(_delayBetweenCharReveal);
        }
        textSlot.text = newText;
        _isReading = false;
        if (pSkipAfter)
        {
            yield return new WaitForSeconds(skipTime);
            ReadLine();
        }
    }

    public void StartCustomLine(string pLine, int pIdLeft, int pIdRight = 0)
    {
        _lines.Add(pIdLeft + itemSep.ToString() + pIdRight + itemSep.ToString() + pLine);
        Debug.Log(pIdLeft + itemSep.ToString() + pIdRight + itemSep.ToString() + pLine);
        
        if (isEmpty)
        {
            isEmpty = false;
            
            leftCharSlot.enabled = true;
            rightCharSlot.enabled = true;
            leftCharSlot.transform.parent.gameObject.SetActive(true);
            rightCharSlot.transform.parent.gameObject.SetActive(true);
            textSlot.enabled = true;
        
            leftCharSlot.color = Color.clear;
            rightCharSlot.color = Color.clear;
            textSlot.text = "";
            
            if(!_isReading)
            {
                ReadLine();
            }
            else
            {
                _isReading = false;
            }
            
            /*if(pAutoRead)
                StartCoroutine(ReadAll(autoReadDelay));*/
        }
    }

    public void StartSingleLine(string pFile, string pAltColor = "blue", string pNormalColor = "black")
    {
        normalColor = pNormalColor;
        altColor = pAltColor;
        
        _skipAfter = true;
        
        // Loading text file
        TextAsset txtAsset = Resources.Load("Dialog/" + pFile) as TextAsset;
        string rawTxt = "";
        if(txtAsset != null)
            rawTxt = txtAsset.ToString();
        
        // Read only the first line
        string line = rawTxt.Split(lineSep)[0];
        _lines.Clear();
        if(_slowReadCoroutine != null)
            StopCoroutine(_slowReadCoroutine);
        
        // Formating the lines in a list
        string[] tmpLines = rawTxt.Split(lineSep);
        
        // Adding each line to _lines (except the last, which is empty
        for(int i = 0 ; i < tmpLines.Length ; i++)
            if (tmpLines[i].Length > 4)
            {
                _lines.Add(tmpLines[i]);
            }

        isEmpty = false;
        
        leftCharSlot.enabled = true;
        rightCharSlot.enabled = true;
        textSlot.enabled = true;
        skipKey.enabled = false;
        
        panel.SetActive(true);

        leftCharSlot.color = Color.clear;
        rightCharSlot.color = Color.clear;
        textSlot.text = "";

        _isReading = false;
        
        if(!_isReading)
        {
            ReadLine();
        }
        else
        {
            _isReading = false;
        }
    }
    
    IEnumerator SkipAll()
    {
        while (_lines.Count > 0)
        {
            yield return null;
            if(_slowReadCoroutine != null)
                StopCoroutine(_slowReadCoroutine);
            _isReading = false;
            ReadLine();
            
        }
        if(_slowReadCoroutine != null)
            StopCoroutine(_slowReadCoroutine);
        
        _isReading = false;
        ReadLine();
    }

    private void OnControllerTypeChanged()
    {
        _actualController = InputManager.GetController();
        switch (_actualController)
        {
            case Controller.Playstation:
                _originalSkipKey = originalKeys[1];
                _altSkipKey = originalKeys[1];
                skipKey.sprite = _originalSkipKey;
                break;
            case Controller.Xbox:
                _originalSkipKey = originalKeys[2];
                _altSkipKey = originalKeys[2];
                skipKey.sprite = _originalSkipKey;
                break;
            case Controller.Other:
                _originalSkipKey = originalKeys[0];
                _altSkipKey = originalKeys[0];
                skipKey.sprite = _originalSkipKey;
                break;
        }
        Debug.Log("ControllerTypeChanged Called From Dialog");
        Debug.Log(_actualController);
    }

    public void StartEndDialogue(string pFile)
    {
        isEnd = true;
        normalColor = "black";
        altColor = "blue";
        
        // Loading text file
        TextAsset txtAsset = Resources.Load("Dialog/" + pFile) as TextAsset;
        string rawTxt = "";
        if(txtAsset != null)
            rawTxt = txtAsset.ToString();
        
        // Read only the first line
        string line = rawTxt.Split(lineSep)[0];
        _lines.Clear();
        if(_slowReadCoroutine != null)
            StopCoroutine(_slowReadCoroutine);
        
        // Formating the lines in a list
        string[] tmpLines = rawTxt.Split(lineSep);
        
        // Adding each line to _lines (except the last, which is empty
        for(int i = 0 ; i < tmpLines.Length ; i++)
            if (tmpLines[i].Length > 4)
            {
                _lines.Add(tmpLines[i]);
            }

        isEmpty = false;
        
        leftCharSlot.enabled = true;
        rightCharSlot.enabled = true;
        textSlot.enabled = true;
        skipKey.enabled = false;
        
        panel.SetActive(true);

        leftCharSlot.color = Color.clear;
        rightCharSlot.color = Color.clear;
        textSlot.text = "";

        _isReading = false;
        
        StartCoroutine(dialogueEndCoroutine());
    }

    IEnumerator dialogueEndCoroutine()
    {
        endMusic.clip = endClip;
        endMusic.Stop();
        for (int i = 0; i < 5; i++)
        {
            ReadLine(false);
            yield return new WaitForSeconds(2 * (_endTime[0+1] - _endTime[0]));
        }
        endMusic.Play();
        OnFinalDialogMusicStart?.Invoke();
        for (int i = 0 ; i < 6 ; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                endMusic.time = _endTime[i];
                if(j == 0)
                    ReadLine(true);
                yield return new WaitForSeconds(_endTime[i+1] - _endTime[i]);
            }
        }
        
        endMusic.time = _endTime[6];
        ReadLine(true);
        OnFinalDialog?.Invoke();
        yield return new WaitForSeconds(_endTime[6+1] - _endTime[6] - 2);
        
        _isReading = false;
        ReadLine(true);
    }
}
