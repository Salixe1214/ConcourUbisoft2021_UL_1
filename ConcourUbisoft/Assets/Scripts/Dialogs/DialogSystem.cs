using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] private TextAsset rawText;
    [SerializeField] private char lineSep = '\n';
    [SerializeField] private char itemSep = ',';
    private List<string> _character1 = new List<string>();
    private List<string> _character2 = new List<string>();
    private List<string> _line = new List<string>();
    private int _lineCount = 0;
    private int _numLines = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        string text = rawText.ToString();

        string[] lines = text.Split(lineSep);

        for(var i = 0 ; i < lines.Length - 1 ; i++)
        {
            string[] line = lines[i].Split(itemSep);
            
            Debug.Log("Line " + i + ": " + line[0]);

            _character1.Insert(i, line[0]);
            _character2.Insert(i, line[1]);
            _line.Insert(i, line[2]);
        }

        _numLines = lines.Length - 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("_numlines: " + _numLines + "\n_lineCount: " + _lineCount);
            Debug.Log("Perso 1: " + _character1[_lineCount]);
            Debug.Log("Perso 2: " + _character2[_lineCount]);
            Debug.Log("Message: " + _line[_lineCount]);

            if(_lineCount < _numLines - 1)
                _lineCount += 1;
        }
    }
}
