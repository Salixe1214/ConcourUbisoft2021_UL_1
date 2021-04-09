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
        private RadioGroup _group;
        private TextColor _text;
        [SerializeField] private int value;

        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                if (!value && isOn)
                {
                    graphic.color = colors.disabledColor;
                }
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            RadioButton[] me = {this};
            _text = GetComponentInChildren<TextColor>();
            _group = group.GetComponent<RadioGroup>();
            onValueChanged.AddListener(OnValueChange);
        }

        private void OnValueChange(bool newValue)
        {
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
            if (isOn || !interactable) return;
            _text.OnSelectedEnter();
            graphic.color = colors.selectedColor;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (isOn || !interactable) return;
            _text.OnSelectedExit();
            graphic.color = colors.normalColor;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (!interactable) return;
            graphic.color = colors.highlightedColor;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (!interactable) return;
            if (!isOn)
            {
                graphic.color = colors.normalColor;
            }
            else
            {
                graphic.color = colors.selectedColor;
            }
        }

        public void OnSelected()
        {
            _text.OnSelectedEnter();
        }

        public void OnDeselected()
        {
            _text.OnSelectedExit();
        }

        public int GetValue()
        {
            return value;
        }
   }
}
