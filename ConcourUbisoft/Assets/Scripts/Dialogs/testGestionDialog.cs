using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGestionDialog : MonoBehaviour
{
    [SerializeField] private DialogSystem _dialogSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        //_dialogSystem.InitiateDialog(null, '\n', ',');
        _dialogSystem.StartDialog();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
