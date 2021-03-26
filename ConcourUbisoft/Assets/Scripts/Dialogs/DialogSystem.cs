using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    // UI components
    [SerializeField] private Image leftCharSlot;  //< Character slot at the left
    [SerializeField] private Image rightCharSlot; //< Character slot at the right
    [SerializeField] private Text textSlot;       //< Text slot
    [SerializeField] private GameObject panel = null;

    // Sprites
    [SerializeField] private Sprite char1Sprite;
    [SerializeField] private Sprite char2Sprite;
    
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

    private NetworkController _networkController = null;

    private void Awake()
    {
        rightCharSlot.transform.localRotation = Quaternion.Euler(0,180,0);
        leftCharSlot.enabled = false;
        rightCharSlot.enabled = false;
        textSlot.enabled = false;

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
        if (Input.GetButtonDown("Submit"))
        {
            ReadLine();
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
                    break;
                case 1:
                    leftCharSlot.sprite = char1Sprite;
                    leftCharSlot.color = Color.white;
                    break;
                case 2:
                    leftCharSlot.sprite = char2Sprite;
                    leftCharSlot.color = Color.white;
                    break;
                default:
                    leftCharSlot.sprite = null;
                    leftCharSlot.color = Color.clear;
                    break;
            }
            
            // Right slot
            switch (rightCharacterID)
            {
                case 0:
                    rightCharSlot.sprite = null;
                    rightCharSlot.color = Color.clear;
                    break;
                case 1:
                    rightCharSlot.sprite = char1Sprite;
                    rightCharSlot.color = Color.white;
                    break;
                case 2:
                    rightCharSlot.sprite = char2Sprite;
                    rightCharSlot.color = Color.white;
                    break;
                default:
                    rightCharSlot.sprite = null;
                    rightCharSlot.color = Color.clear;
                    break;
            }

            textSlot.text = parsedLine[2];
        }
        else
        {
            leftCharSlot.color = Color.clear;
            leftCharSlot.enabled = false;
            
            rightCharSlot.color = Color.clear;
            rightCharSlot.enabled = false;
            
            textSlot.text = "";
            textSlot.enabled = false;
            panel.SetActive(false);

            isEmpty = true;
        }
    }
    
    public void StartDialog(string pFile,  bool pAutoRead = false, int pTimeBetweenLines = 3)
    {
        // Loading text file
        TextAsset txtAsset = Resources.Load("Dialog/" + pFile) as TextAsset;
        string rawTxt = "";
        if(txtAsset != null)
            rawTxt = txtAsset.ToString();
        
        // Formating the lines in a list
        string[] tmpLines = rawTxt.Split(lineSep);
        // Adding each line to _lines (except the last, which is empty
        for(int i = 0 ; i < tmpLines.Length - 1 ; i++)
            _lines.Add(tmpLines[i]);
        
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
            
            ReadLine();
            
            if(pAutoRead)
                StartCoroutine(ReadAll(pTimeBetweenLines));
        }
    }
    
    public void StartCustomLine(string pLine, int pIdLeft, int pIdRight = 0, bool pAutoRead = false, int pTimeBetweenLines = 3)
    {
        _lines.Add(pIdLeft + itemSep.ToString() + pIdRight + itemSep.ToString() + pLine);
        Debug.Log(pIdLeft + itemSep.ToString() + pIdRight + itemSep.ToString() + pLine);
        
        if (isEmpty)
        {
            isEmpty = false;
            
            leftCharSlot.enabled = true;
            rightCharSlot.enabled = true;
            textSlot.enabled = true;
        
            leftCharSlot.color = Color.clear;
            rightCharSlot.color = Color.clear;
            textSlot.text = "";
            
            ReadLine();
            
            if(pAutoRead)
                StartCoroutine(ReadAll(pTimeBetweenLines));
        }
    }

    IEnumerator ReadAll(int pTimeBetweenLines)
    {
        while (!isEmpty)
        {
            yield return new WaitForSeconds(5);
            ReadLine();
        }
    }
}
