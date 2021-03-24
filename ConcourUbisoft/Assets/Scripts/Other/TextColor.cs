using System;
using UnityEngine;
using UnityEngine.UI;

namespace Other
{
    [RequireComponent(typeof(Text))]
    public class TextColor : MonoBehaviour
    {
        private Text _text;
        private Color _defaultColor;
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
            _text.color = onChangeColor;
        }

        public void OnSelectedExit()
        {
            _text.color = _defaultColor;
        }
    }
}
