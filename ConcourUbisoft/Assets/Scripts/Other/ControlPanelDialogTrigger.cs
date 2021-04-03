using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelDialogTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private DialogSystem _dialogSystem;
    private BoxCollider _collider;
    void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("PlayerGuard"))
        {
            _dialogSystem.StartDialog("Area02_first_control");
            _collider.gameObject.SetActive(false);
        }
        
    }
}
