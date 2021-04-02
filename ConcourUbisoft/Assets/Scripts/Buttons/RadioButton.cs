using System;
using System.Collections.Generic;
using System.Linq;
using Other;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Buttons
{
    [RequireComponent(typeof(Image))]
    public class RadioButton : Toggle
    {
        private IEnumerable<RadioButton> _buttons;
        private RadioGroup _group;
        private TextColor _text;
        [SerializeField] private int value;
        
        protected override void Awake()
        {
            base.Awake();
            RadioButton[] me = {this};
            _text = GetComponentInChildren<TextColor>();
            _group = group.GetComponent<RadioGroup>();
            _buttons = _group.gameObject.GetComponentsInChildren<RadioButton>().Except(me);
            onValueChanged.AddListener(OnValueChange);
        }

        private void OnValueChange(bool newValue)
        {
            Debug.Log("OnValueChanged called");
            if (newValue)
            {
                OnSelected();
                _group.Set(value);
            }
            else
            {
                OnDeselected();
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (isOn) return;
            _text.OnSelectedEnter();
            graphic.color = colors.highlightedColor;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (isOn) return;
            _text.OnSelectedExit();
            graphic.color = colors.normalColor;
        }

        private void OnSelected()
        {
            foreach (RadioButton button in _buttons)
            {
                button.isOn = false;
            }
            _text.OnSelectedEnter();
        }

        private void OnDeselected()
        {
            _text.OnSelectedExit();
        }
    }
}
