using System;
using System.Collections.Generic;
using Inputs;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace TechSupport.Controller
{
    [RequireComponent(typeof(Controllable), typeof(Outline))]
    public class ControllableOutline : MonoBehaviour
    {
        // Game Flow Object
        private GameController _gameController;
        private NetworkController _networkController;

        private Controllable _controllable;
        private Outline _outline;
        private RawImage _image;
        private InputManager _inputManager;

        private bool _wasControlled = false;

        [Header("Outline")]
        [SerializeField] private Color outlineColor = Color.red;
        [SerializeField, Range(0f, 10f)] private float outlineWidth = 10f;

        [Header("Button Position")] 
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject targetTop;
        
        [Header("Inputs")]
        [SerializeField] private string inputName = "Control";
        [SerializeField] [NotNull] private GameObject controls;
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
            _networkController = GameObject.FindGameObjectWithTag("NetworkController")?.GetComponent<NetworkController>();
            _inputManager = GameObject.FindWithTag("InputManager")?.GetComponent<InputManager>();
            SetupOutline();
            SetupInputImage();
            enabled = false;
            _outline.enabled = false;
            _image.enabled = false;
        }
        
        private void OnEnable()
        {
            _inputManager.OnControllerTypeChanged += OnControllerChanged;
        }
        private void OnDisable()
        {
            _inputManager.OnControllerTypeChanged -= OnControllerChanged;
        }

        #region Object Setup

        private void SetupOutline()
        {
            _outline = GetComponent<Outline>();
            _outline.OutlineColor = outlineColor;
            _outline.OutlineWidth = outlineWidth;
            _outline.OutlineMode = Outline.Mode.OutlineAll;
        }

        private void SetupInputImage()
        {
            _image = new GameObject().AddComponent<RawImage>();
            Transform imageTransform = _image.gameObject.transform;

            _image.texture = textures[InputManager.GetController()];
            imageTransform.SetParent(canvas.transform);
            imageTransform.localScale /= 2;
            imageTransform.localPosition = Vector3.zero;
        }

        #endregion

        #region Game Flow

        
        private bool GameMenuOpen()
        {
            return _gameController != null && _gameController.IsGameMenuOpen;
        }

        private bool NetworkOwnerInvalid()
        {
            return _networkController != null && _networkController.GetLocalRole() != GameController.Role.Technician;
        }

        #endregion
        
        private void UpdateOutline()
        {
            _outline.enabled = !_controllable.IsControlled;
            _image.enabled = !_controllable.IsControlled;
        }

        private void Update()
        {
            if (GameMenuOpen() || NetworkOwnerInvalid())
                return;

            if (Input.GetButtonUp(InputManager.GetInputNameByController(inputName)))
            {
                _controllable.IsControlled = !_controllable.IsControlled;
                controls.SetActive(_controllable.IsControlled);
                UpdateOutline();
            }
        }

        private void FixedUpdate()
        {
            if (FullScreenSystem.Current == null)
                return;
            _image.gameObject.transform.position = FullScreenSystem.Current.WorldToScreenPoint(targetTop.transform.position);
        }

        private void OnControllerChanged()
        {
            _image.texture = textures[InputManager.GetController()];
        }

        public void Enable(bool enabledOutline, Camera c)
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
            controls.SetActive(enabledOutline && _controllable.IsControlled);
        }
    }
}
