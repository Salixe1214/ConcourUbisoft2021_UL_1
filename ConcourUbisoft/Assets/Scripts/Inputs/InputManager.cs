using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inputs
{
    public enum Controller
    {
        Xbox,
        Playstation,
        Other
    }
    
    public class InputManager : MonoBehaviour
    {
        public event Action OnControllerTypeChanged;

        private Controller _previousController;
        
        private static readonly Dictionary<Tuple<Controller, string>, string> Commands = new Dictionary<Tuple<Controller, string>, string>()
        {
            { new Tuple<Controller, string>(Controller.Xbox, "Control"), "ControlXbo"},
            { new Tuple<Controller, string>(Controller.Playstation, "Control"), "ControlPS"},
            { new Tuple<Controller, string>(Controller.Other, "Control"), "Control"},
        };
        
        private static Controller _controller = Controller.Other;
        
        private void Awake()
        {
            _previousController = Controller.Other;
            SearchForController();
            Debug.Log("Awake Controller type");
            Debug.Log(_controller);
            Debug.Log("Awake previous controller type");
            Debug.Log(_previousController);
        }

        private void Update()
        {
            SearchForController();

            if (_previousController != _controller)
            {
                OnControllerTypeChanged?.Invoke();
                Debug.Log("Controller type changed");
                _previousController = _controller;
            }
        }
        

        private static void SearchForController()
        {
            IEnumerable<string> joysticks = Input.GetJoystickNames(); 
            
            if (joysticks.Contains("Controller (Xbox One For Windows)"))
            {
                _controller = Controller.Xbox;
            }
            else if (joysticks.Contains("Wireless Controller"))
            {
                _controller = Controller.Playstation;
            }
            else
            {
                _controller = Controller.Other;
            }
        }

        public static Controller GetController()
        {
            return _controller;
        }

        public static string GetInputNameByController(string inputName)
        {
            SearchForController();
            return Commands[new Tuple<Controller, string>(_controller, inputName)];
        }
    }
}
