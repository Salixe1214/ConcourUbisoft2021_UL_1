using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Inputs
{
    public class TargetInput : MonoBehaviour
    {
        private InputManager _inputManager;
        [SerializeField] public SerializableDictionary<Controller, GameObject> targets 
                = new SerializableDictionary<Controller, GameObject>(
                    new Dictionary<Controller, GameObject> {
                        { Controller.Xbox, null },
                        { Controller.Playstation, null },
                        { Controller.Other, null }
                    });

        private void Awake()
        {
            _inputManager = GameObject.FindWithTag("InputManager")?.GetComponent<InputManager>();
            foreach (KeyValuePair<Controller,GameObject> target in targets)
            {
                target.Value.SetActive(false);
            }
            targets[InputManager.GetController()].SetActive(true);
        }

        private void OnEnable()
        {
            _inputManager.OnControllerTypeChanged += OnControllerChanged;
        }
        
        private void OnDisable()
        {
            _inputManager.OnControllerTypeChanged -= OnControllerChanged;
        }
        
        private void OnControllerChanged()
        {
            foreach (KeyValuePair<Controller,GameObject> target in targets)
            {
                target.Value.SetActive(false);
            }
            targets[InputManager.GetController()].SetActive(true);
        }
    }
}
