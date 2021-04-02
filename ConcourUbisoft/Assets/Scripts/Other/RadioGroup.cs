using System;
using System.Collections.Generic;
using Buttons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Other
{
    public class RadioGroup : ToggleGroup
    {
        [Serializable]
        public class OptionData
        {

            [SerializeField]
            private string text;
            [SerializeField]
            private RadioButton button;

            public string Text  { get => text; set => text = value; }

            public RadioButton Button { get => button; set => button = value; }

            public OptionData()
            {
            }

            public OptionData(string text)
            {
                this.Text = text;
            }

            public OptionData(RadioButton button)
            {
                this.Button = button;
            }

            public OptionData(string text, RadioButton button)
            {
                this.Text = text;
                this.Button = button;
            }            
        }
        [Serializable]
        public class RadioEvent : UnityEvent<int> {}

        [SerializeField] private List<OptionData> options = new List<OptionData>();
        
        [Space]

        [SerializeField]
        private RadioEvent onValueChanged = new RadioEvent();
        public int Value { get; private set; } = 0;

        public void Set(int value, bool sendCallback = true)
        {
            if (Application.isPlaying && (value == Value || options.Count == 0))
                return;

            Value = Mathf.Clamp(value, 0, options.Count - 1);

            if (sendCallback)
            {
                onValueChanged.Invoke(Value);
            }
        }
    }
}
