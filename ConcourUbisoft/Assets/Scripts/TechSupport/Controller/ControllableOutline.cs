using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        private enum ControllerType
        {
            Xbox,
            Playstation,
            Other
        }

        private ControllerType _controllerType = ControllerType.Other;

        [Header("Outline")]
        [SerializeField] private GameObject outlineTarget;
        [SerializeField] private Color outlineColor = Color.white;
        [SerializeField, Range(0f, 10f)] private float outlineWidth = 10f;

        [Header("Button Position")] 
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject targetTop;

        [Header("Default Input")]
        [SerializeField] private string defaultInputName = "Control";
        [SerializeField] private Texture defaultInputSprite = null;
        
        [Header("Xbox Input")]
        [SerializeField] private string xboxInputName = "ControlXBO";
        [SerializeField] private Texture xboxInputSprite = null;

        [Header("PS Input")]
        [SerializeField] private string psInputName = "ControlPS";
        [SerializeField] private Texture psInputSprite = null;

        void Awake()
        {
            _controllable = GetComponent<Controllable>();
            _gameController = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameController>();
            _outline = outlineTarget.AddComponent<Outline>();

            _outline.OutlineColor = outlineColor;
            _outline.OutlineWidth = outlineWidth;
            _outline.OutlineMode = Outline.Mode.OutlineAll;
            
            _image = (new GameObject()).AddComponent<RawImage>();
            _image.texture = defaultInputSprite;
            _image.gameObject.transform.SetParent(canvas.transform);
            _image.gameObject.transform.localScale /= 2;
            Enable(false);
        }

        private void UpdateOutline()
        {
            _outline.enabled = !_controllable.IsControlled;
            _image.gameObject.SetActive(!_controllable.IsControlled);
        }

        private void Update()
        {
            if (_gameController != null && _gameController.IsGameMenuOpen)
                return;
            
            if (Input.GetButtonUp(defaultInputName)
                || Input.GetButtonUp(xboxInputName)
                || Input.GetButtonUp(psInputName))
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
            _outline.enabled = enabledOutline;
            _image.enabled = enabledOutline;
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
        }
    }
}
