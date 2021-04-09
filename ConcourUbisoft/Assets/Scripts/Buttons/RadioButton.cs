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
                ColorBlock newColorBlock = colors;
                newColorBlock.disabledColor = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1f);
                colors = newColorBlock;
                //graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1f);
            }
            else
            {
                ColorBlock newColorBlock = colors;
                newColorBlock.disabledColor = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0f);
                colors = newColorBlock;
                //graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0f);
                OnDeselected();
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
