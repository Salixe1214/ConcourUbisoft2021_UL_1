using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using UnityEngine;
using UnityEngine.Events;
namespace Arm
{
    public class MechanicalArmWASDControl : Controllable
    {
        [SerializeField] private ArmController _armController = null;
        [SerializeField] private MagnetController _magnetController = null;
        [SerializeField] private GameController.Role _owner = GameController.Role.SecurityGuard;

        private NetworkController _networkController = null;
        private InputManager _inputManager;
        private Controller _currentController;


        private void Awake()
        {
            _networkController = GameObject.FindGameObjectWithTag("NetworkController")?.GetComponent<NetworkController>();
            _inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        }

        private void OnEnable()
        {
            _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
        }

        private void OnDisable()
        {
            _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
        }

        private void Update()
        {
            if (IsControlled && (_owner == GameController.Role.None || _owner == _networkController.GetLocalRole())) {
                Vector3 translation = new Vector3(-Input.GetAxisRaw("Vertical"), 0, Input.GetAxisRaw("Horizontal"));

                _armController.Translate(translation);

                _currentController = InputManager.GetController();
                Debug.Log(_currentController + "Arm");
                if (((Input.GetButtonUp("Grab") && _currentController == Controller.Other)||
                     (Input.GetButtonUp("GrabControllerXBO") && _currentController == Controller.Xbox) ||
                     (Input.GetButtonUp("GrabControllerPS")&& _currentController == Controller.Playstation)))
                {
                    Debug.Log("Toggle Magnet");
                    _magnetController.MagnetActive = !_magnetController.MagnetActive;
                }
                
            }
        }

        private void OnControllerTypeChanged()
        {
            _currentController = InputManager.GetController();
        }
    }
}
