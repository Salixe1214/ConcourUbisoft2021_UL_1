using System;
using UnityEngine;
using UnityEngine.UI;

namespace Other
{
    [RequireComponent(typeof(Text))]
    public class TextColor : MonoBehaviour
    {
        private Text _text;
        private Color _defaultColor = new Color(255,255,255,255);
        [SerializeField] private Color onChangeColor;

        private void Awake()
        {
            _text = GetComponent<Text>();
            _defaultColor = _text.color;
        }

        public void OnHover()
        {
            _text.color = onChangeColor;
        }
        public void OnExit()
        {
            _text.color = _defaultColor;
        }

        public void OnSelectedEnter()
        {
            if (_text)
                _text.color = onChangeColor;
        }

        public void OnSelectedExit()
        {
            if (_text)
                _text.color = _defaultColor;
        }
    }
}
