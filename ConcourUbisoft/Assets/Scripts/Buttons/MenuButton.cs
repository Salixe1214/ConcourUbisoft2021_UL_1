using System;
using Inputs;
using Other;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Buttons
{
    [RequireComponent(typeof(Selectable))]
    public class MenuButton : EventTrigger
    {
        private InputManager _inputManager;
        private Selectable _button;
        private TextColor _coloredText;

        private bool _selected = false;

        private void Awake()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _coloredText = GetComponentInChildren<TextColor>();
            _button = GetComponent<Selectable>();
        }

        private void Start()
        { 
            OnControllerTypeChanged();
            if (_selected)
            {
                _coloredText.OnSelectedEnter();
            }
            else
            {
                _coloredText.OnSelectedExit();
            }
        }

        private void OnEnable()
        {
            _inputManager.OnControllerTypeChanged += OnControllerTypeChanged;
        }

        private void OnDisable()
        {
            _inputManager.OnControllerTypeChanged -= OnControllerTypeChanged;
        }

        private void OnControllerTypeChanged()
        {
            if (InputManager.GetController() == Controller.Other)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _coloredText.OnHover();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _coloredText.OnExit();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            _selected = true;
            _coloredText.OnSelectedEnter();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            _selected = false;
            _coloredText.OnSelectedExit();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _coloredText.OnSelectedExit();
            _button.OnDeselect(eventData);
        }
    }
}
