using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Inputs
{
    [RequireComponent(typeof(Image))]
    public class InputImage : MonoBehaviour
    {
        private Image _image;
        
        [SerializeField] public SerializableDictionary<Controller, Sprite> sprites 
                = new SerializableDictionary<Controller, Sprite>(
                    new Dictionary<Controller, Sprite> {
                        { Controller.Xbox, null },
                        { Controller.Playstation, null },
                        { Controller.Other, null }
                    })
            ;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        void Update()
        {
            if (_image.sprite != sprites[InputManager.GetController()])
                _image.sprite = sprites[InputManager.GetController()];
        }
    }
}
