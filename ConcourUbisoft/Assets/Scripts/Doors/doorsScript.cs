using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.Video;

public class doorsScript : MonoBehaviour
{
    private enum Direction { Up, Right, Down, Left, Err }
    [SerializeField] private List<Direction> baseSequence;
    private List<Direction> _sequence;

    [SerializeField] private GameObject indicator;
    private Material _matIndicator;
    private Color _color;
    
    // Start is called before the first frame update
    void Awake()
    {
        _matIndicator = indicator.GetComponent<Renderer>().material;
        _color = _matIndicator.color;
        _sequence = new List<Direction>();
        foreach (var i in baseSequence)
        {
            _sequence.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPressed(string name)
    {
        Direction direction;
        switch (name)
        {
            case "Up":
                direction = Direction.Up;
                break;
            case "Right":
                direction = Direction.Right;
                break;
            case "Down":
                direction = Direction.Down;
                break;
            case "Left":
                direction = Direction.Left;
                break;
            default:
                direction = Direction.Err;
                break;
        }
        
        if(direction == Direction.Err)
            Debug.Log("Erreur, ce bouton ne correspond pas à auccun boutton");
        else if(_sequence.Count > 0)
        {
            if (direction == _sequence[0])
            {
                _matIndicator.SetColor("_Color", Color.green);
                _sequence.RemoveAt(0);
                
                // The door open
                if (_sequence.Count <= 0)
                {
                    OnDoorUnlock();
                }
                
                StartCoroutine(Flash(_color, _matIndicator));
            }
            else
            {
                _matIndicator.SetColor("_Color", Color.red);
                
                // Resetting the sequence
                _sequence.Clear();
                foreach (var i in baseSequence)
                {
                    _sequence.Add(i);
                }
                
                StartCoroutine(Flash(_color, _matIndicator));
            }
        }
    }

    private void OnDoorUnlock()
    {
        _color = Color.magenta;
        
        Debug.Log("Unlocked");

        GetComponent<Collider>().isTrigger = true;
    }

    IEnumerator Flash(Color pColor, Material pMaterial)
    {
        yield return new WaitForSeconds(0.33f);
        pMaterial.SetColor("_Color", pColor);
    }
}
