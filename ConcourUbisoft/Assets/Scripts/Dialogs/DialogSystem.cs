using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Inputs;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    // UI components
    [SerializeField] private Image leftCharSlot;  //< Character slot at the left
    [SerializeField] private Image rightCharSlot; //< Character slot at the right
    [SerializeField] private Text textSlot;       //< Text slot
    [SerializeField] private GameObject panel = null;
    [SerializeField] private float _delayBetweenCharReveal = 0.1f;
    [SerializeField] private Sprite[] originalKeys;
    [SerializeField] private Sprite altKey;
    [SerializeField] private Image skipKey;
    private Sprite _altSkipKey;
    private Sprite _originalSkipKey;

    // Sprites
    [SerializeField] private Sprite char1Sprite;
    [SerializeField] private Sprite char2Sprite;
    [SerializeField] private Sprite char3Sprite;
    
    // Line separators
    [SerializeField] private char lineSep = '\n';
    [SerializeField] private char itemSep = ';';
    
    private List<string> _lines = new List<string>(); //< Contains all the lines (and the informations of the characters shown)
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
     * 1 or 2 for character 1 or 2 (defined by the sprites)
     * 0 for none
     */

    private bool isEmpty = true;
    private bool _isReading = false;

    private NetworkController _networkController = null;
    private AudioSource _audioSource = null;

    // Long press parameters
    [SerializeField] private float longPressDuration = 1;
    private bool bIsPressed = false;
    private float bDownTime = 0;

    private IEnumerator _slowReadCoroutine;
    private Controller _actualController;

    private void Awake()
    {

        _originalSkipKey = originalKeys[0];
        _altSkipKey = altKey;
        skipKey.sprite = _originalSkipKey;
        _audioSource = GetComponent<AudioSource>();

        rightCharSlot.transform.localRotation = Quaternion.Euler(0,180,0);
        leftCharSlot.enabled = false;
        rightCharSlot.enabled = false;
        textSlot.enabled = false;
        // textSlot.resizeTextForBestFit = true; //< Have a nice effect but math doesn't like it. Yeah I dont -Mat

        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();

        if(_networkController.GetLocalRole() == GameController.Role.SecurityGuard)
        {
            var temp = leftCharSlot;
            leftCharSlot = rightCharSlot;
            rightCharSlot = temp;
        }
    }

    void Update()
    {
        KeyCode key;
        if (_actualController != InputManager.GetController())
        {
            switch (InputManager.GetController())
            {
                case Controller.Playstation:
                    _originalSkipKey = originalKeys[1];
                    _altSkipKey = originalKeys[1];
                    break;
                case Controller.Xbox:
                    _originalSkipKey = originalKeys[2];
                    _altSkipKey = originalKeys[2];
                    break;
                default:
                    _originalSkipKey = originalKeys[0];
                    _altSkipKey = altKey;
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            bDownTime = 0;
            bIsPressed = true;
            
        }

        if (bIsPressed && (Input.GetKey(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button4)))
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

        if (bIsPressed && (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button4)))
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

    public void ReadLine()
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
                    leftCharSlot.sprite = null;
                    leftCharSlot.color = Color.clear;
                    leftCharSlot.transform.parent.gameObject.SetActive(false);
                    break;
                case 1:
                    leftCharSlot.sprite = char1Sprite;
                    leftCharSlot.color = Color.white;
                    leftCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 2:
                    leftCharSlot.sprite = char2Sprite;
                    leftCharSlot.color = Color.white;
                    leftCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 3:
                    leftCharSlot.sprite = char3Sprite;
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
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0,180,0);
                    rightCharSlot.sprite = null;
                    rightCharSlot.color = Color.clear;
                    rightCharSlot.transform.parent.gameObject.SetActive(false);
                    break;
                case 1:
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0,180,0);
                    rightCharSlot.sprite = char1Sprite;
                    rightCharSlot.color = Color.white;
                    rightCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 2:
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0,180,0);
                    rightCharSlot.sprite = char2Sprite;
                    rightCharSlot.color = Color.white;
                    rightCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                case 3:
                    rightCharSlot.transform.localRotation = Quaternion.Euler(0,0,0);
                    rightCharSlot.sprite = char3Sprite;
                    rightCharSlot.color = Color.white;
                    rightCharSlot.transform.parent.gameObject.SetActive(true);
                    break;
                default:
                    rightCharSlot.sprite = null;
                    rightCharSlot.color = Color.clear;
                    rightCharSlot.transform.parent.gameObject.SetActive(false);
                    break;
            }

            _slowReadCoroutine = SlowRead(parsedLine[2]);
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
    
    public void StartDialog(string pFile)
    {
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
    
    IEnumerator SlowRead(string text)
    {
        _isReading = true;
        int i = 0;
        while (i < text.Length && _isReading)
        {
            textSlot.text = text.Substring(0, i + 1);
            if (_audioSource.isPlaying == false)
            {
                _audioSource.Play();
            }
            
            i++;
            yield return new WaitForSeconds(_delayBetweenCharReveal);
        }
        textSlot.text = text;
        _isReading = false;
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

    /*IEnumerator ReadAll(float pTimeBetweenLines)
    {
        while (!isEmpty)
        {
            yield return new WaitForSeconds(pTimeBetweenLines);
            if(_lines.Count > 0)
            {
                ReadLine();
            }
            else
            {
                _isReading = false;
            }
        }
    }*/
    
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
    }
}
