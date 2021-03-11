using System;
using System.Collections.Generic;
using Inputs;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace TechSupport.Controller
{
    [RequireComponent(typeof(Controllable))]
    public class ControllableOutline : MonoBehaviour
    {
        private Controllable _controllable;
        private Outline _outline;
        private RawImage _image;
        private GameController _gameController = null;
        
        private bool _wasControlled = false;

        [Header("Outline")]
        [SerializeField] private GameObject outlineTarget;
        [SerializeField] private Color outlineColor = Color.white;
        [SerializeField, Range(0f, 10f)] private float outlineWidth = 10f;

        [Header("Button Position")] 
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject targetTop;
        
        [Header("Inputs")]
        [SerializeField] private string inputName = "Control";
        [SerializeField] public SerializableDictionary<Inputs.Controller, Texture> textures 
            = new SerializableDictionary<Inputs.Controller, Texture>(
            new Dictionary<Inputs.Controller, Texture> {
                { Inputs.Controller.Xbox, null },
                { Inputs.Controller.Playstation, null },
                { Inputs.Controller.Other, null }
            })
;

        void Awake()
        {
            _controllable = GetComponent<Controllable>();
            _gameController = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameController>();
            _outline = outlineTarget.AddComponent<Outline>();

            _outline.OutlineColor = outlineColor;
            _outline.OutlineWidth = outlineWidth;
            _outline.OutlineMode = Outline.Mode.OutlineAll;
            
            _image = (new GameObject()).AddComponent<RawImage>();
            _image.texture = textures[InputManager.GetController()];
            _image.gameObject.transform.SetParent(canvas.transform);
            _image.gameObject.transform.localScale /= 2;
            Enable(false);
        }

        private void UpdateOutline()
        {
            _outline.enabled = !_controllable.IsControlled;
            _image.enabled = !_controllable.IsControlled;
        }

        private void Update()
        {
            if (_gameController != null && _gameController.IsGameMenuOpen)
                return;
            
            if (Input.GetButtonUp(InputManager.GetInputNameByController(inputName)))
            {
                _controllable.IsControlled = !_controllable.IsControlled;
                UpdateOutline();
            }
        }

        private void FixedUpdate()
        {
            _image.gameObject.transform.localPosition = targetTop.transform.position * 10;
        }
        
        public void Enable(bool enabledOutline)
        {
            enabled = enabledOutline;
            if (enabledOutline)
            {
                _controllable.IsControlled = _wasControlled;
            }
            else
            {
                _wasControlled = _controllable.IsControlled;
                _controllable.IsControlled = false;
            }
            _outline.enabled = enabledOutline && !_controllable.IsControlled;
            _image.enabled = enabledOutline && !_controllable.IsControlled;
        }
    }
}
