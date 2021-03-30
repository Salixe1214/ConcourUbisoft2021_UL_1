using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Buttons
{
    [RequireComponent(typeof(Image))]
    public class OutlineButton : UnityEngine.UI.Button
    {        
        protected override void Awake()
        {
            Color c = Color.white;

            c.a = 255;
            image = GetComponent<Image>();
            image.color = c;
            image.type = Image.Type.Sliced;
        }
    }
}
