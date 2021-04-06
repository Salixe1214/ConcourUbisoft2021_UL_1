using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheats : MonoBehaviour
{
    private DialogSystem _dialogSystem;
    // Start is called before the first frame update
    void Start()
    {
        _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartEndDialogue("Area03_end_part2");
        }
        if (Input.GetKeyDown(KeyCode.F2) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartDialog("Area01_start");
        }
        if (Input.GetKeyDown(KeyCode.F3) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartDialog("Area01_initial_arm_control");
        }
        if (Input.GetKeyDown(KeyCode.F4) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartDialog("Area01_end");
        }
        if (Input.GetKeyDown(KeyCode.F5) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartDialog("Area02_start");
        }
        if (Input.GetKeyDown(KeyCode.F6) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartDialog("Area02_initial_arm_control");
        }
        if (Input.GetKeyDown(KeyCode.F7) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartDialog("Area02_first_sequence_done");
        }
        if (Input.GetKeyDown(KeyCode.F8) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartDialog("Area02_second_sequence_done");
        }
        if (Input.GetKeyDown(KeyCode.F9) && Input.GetKey(KeyCode.LeftControl))
        {
            _dialogSystem.StartDialog("Area02_end");
        }
    }
}
