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
            { new Tuple<Controller, string>(Controller.Xbox, "Control"), "ControlXBO"},
            { new Tuple<Controller, string>(Controller.Playstation, "Control"), "ControlPS"},
            { new Tuple<Controller, string>(Controller.Other, "Control"), "Control"},
            { new Tuple<Controller, string>(Controller.Xbox, "CameraEscape"), "CameraEscapeXBO"},
            { new Tuple<Controller, string>(Controller.Playstation, "CameraEscape"), "CameraEscapePS"},
            { new Tuple<Controller, string>(Controller.Other, "CameraEscape"), "CameraEscape"},
            { new Tuple<Controller, string>(Controller.Xbox, "Confirm"), "ConfirmXBO"},
            { new Tuple<Controller, string>(Controller.Playstation, "Confirm"), "ConfirmPS"},
            { new Tuple<Controller, string>(Controller.Other, "Confirm"), "Confirm"},
            { new Tuple<Controller, string>(Controller.Xbox, "OpenInfo"), "OpenInfoXBO"},
            { new Tuple<Controller, string>(Controller.Playstation, "OpenInfo"), "OpenInfoPS"},
            { new Tuple<Controller, string>(Controller.Other, "OpenInfo"), "OpenInfo"},
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
            
            if (joysticks.Contains("Controller (Xbox One For Windows)")||joysticks.Contains("Controller (GEM PAD EX)"))
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
