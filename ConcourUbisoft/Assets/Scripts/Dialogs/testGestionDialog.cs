using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGestionDialog : MonoBehaviour
{
    [SerializeField] private DialogSystem _dialogSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        _dialogSystem.StartDialog("Introduction");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire3"))
            _dialogSystem.StartCustomLine("Allo, Est-ce que ca marche?", 1, 2);
    }
}
