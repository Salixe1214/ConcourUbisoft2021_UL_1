using System;
using UnityEngine;

namespace Other
{
    public class Level3Controller :MonoBehaviour
    {
        private DialogSystem _dialogSystem;
        private bool _endButtonPressed;
        private void Start()
        {
            _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
            _endButtonPressed = false;
        }

        public void StartLevel()
        {
            _dialogSystem.StartDialog("Area03_start");
        }

        public void OnButtonPressed()
        {
            if (!_endButtonPressed)
            {
                _dialogSystem.StartDialog("Area03_end");
            }
            _endButtonPressed = true;
        }

    }
}