using System;
using Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    [RequireComponent(typeof(Button))]
    public class InputTriggerButton : MonoBehaviour
    {
        private Button _button;
        [SerializeField] private string input;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Update()
        {
            if (Input.GetButtonUp(InputManager.GetInputNameByController(input)))
                _button.onClick?.Invoke();
        }
    }
}
