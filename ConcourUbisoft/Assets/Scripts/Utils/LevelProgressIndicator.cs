using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressIndicator : MonoBehaviour
{
    enum ControllerID 
    {
        Level1, level2
    };
    
    [SerializeField] private Color defaultColor = Color.black;
    [SerializeField] private Color completedColor = Color.white;
    
    [SerializeField] private List<GameObject> indicatorLights;
    private List<Renderer> _renderers = new List<Renderer>();

    [SerializeField] private FurnaceController _furnace;
    
    private int _numYellowLights = 0;

    private void Awake()
    {
        _furnace.WhenFurnaceConsumedAll.AddListener(onSuccess);
        _furnace.WhenFurnaceConsumeAWholeSequenceWithoutFinishing.AddListener(onSuccess);
        for(int i = 0 ; i < indicatorLights.Count ; i++)
        {
            _renderers.Insert(i, indicatorLights[i].GetComponent<Renderer>());
        }
    }

    private void Update()
    {
        for(int i = 0 ; i < indicatorLights.Count ; i++)
        {
            if(i < _numYellowLights)
            {
                _renderers[i].material.color = Color.yellow;
                _renderers[i].material.SetColor("_EmissionColor", completedColor * 15);
            }
            else
            {
                _renderers[i].material.color = defaultColor;
                _renderers[i].material.SetColor("_EmissionColor", defaultColor * 15);
            }
        }
    }

    public void onSuccess()
    {
        if (_numYellowLights < indicatorLights.Count)
        {
            _numYellowLights += 1;
        }
    }
}
