using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.Video;

public class DoorsScript : MonoBehaviour
{
    public enum ButtonType { Err, Up, Right, Down, Left, Confirm }
    [SerializeField] private List<ButtonType> unlockSequence;
    private List<ButtonType> _sequence;

    [SerializeField] private GameObject indicator;
    private Material _matIndicator, _matConfirmLight;
    private Color _color;

    [SerializeField] private List<DoorsButton> buttonsList;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject confirmLight;

    public static Action DoorUnlock;
    
    // Start is called before the first frame update
    void Awake()
    {
        _matIndicator = indicator.GetComponent<Renderer>().material;
        _matConfirmLight = confirmLight.GetComponent<Renderer>().material;
        _color = _matConfirmLight.color;
        _sequence = new List<ButtonType>();

        foreach (var button in buttonsList)
        {
            button.ButtonPressed += ButtonPressed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPressed(ButtonType bType)
    {
        switch (bType)
        {
            case ButtonType.Err:
                Debug.Log("Erreur, ce bouton ne correspond pas à auccun boutton");
                break;
            case ButtonType.Confirm:
                OnConfirm();
                break;
            default:
                _sequence.Add(bType);
                break;
        }
    }

    private void OnConfirm()
    {
        bool isSequenceGood;
        
        if (_sequence.Count == unlockSequence.Count)
        {
            isSequenceGood = true;
            for (int i = 0; i < unlockSequence.Count; i++)
            {
                if (unlockSequence[i] != _sequence[i])
                    isSequenceGood = false;
            }
        }
        else
        {
            isSequenceGood = false;
        }


        
        if (isSequenceGood)
        {
            _matConfirmLight.SetColor("_Color", Color.green);
                
            // The door open
            OnDoorUnlock();
        }
        else
        {
            _matConfirmLight.SetColor("_Color", Color.red);
                
            // Resetting the sequence
            _sequence.Clear();
        }
                
        StartCoroutine(Flash(_color, _matConfirmLight));
    }

    private void OnDoorUnlock()
    {
        Debug.Log("Unlocked");

        GetComponent<Collider>().isTrigger = true;
        _matIndicator.SetColor("_Color", Color.green);
        
        if(DoorUnlock != null)
            DoorUnlock.Invoke();
    }

    IEnumerator Flash(Color pColor, Material pMaterial)
    {
        Color flashColor = pMaterial.color;
        
        foreach (var button in buttonsList)
        {
            button.clickable = false;
        }
        
        yield return new WaitForSeconds(1f);
        pMaterial.SetColor("_Color", pColor);

        if (flashColor != Color.green)
        {
            foreach (var button in buttonsList)
            { 
                button.clickable = true;
            }
            
        }
    }
}
