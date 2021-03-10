using System;
using System.Collections.Generic;
using UnityEngine;

namespace TechSupport.Controller
{
    public class ControllerSystem : MonoBehaviour
    {
        [Serializable]
        private readonly struct Screen
        {
            public readonly Camera Camera;
            public readonly ControllableOutline Controllable;

            public Screen(Camera camera, ControllableOutline controllable)
            {
                Camera = camera;
                Controllable = controllable;
            }
        }
        [SerializeField] private List<Screen> controllers = null;
        [SerializeField] private Sprite defaultInputSprite = null;
        
        private void Awake()
        {
            controllers.ForEach(screen =>
            {
                if (screen.Camera.enabled)
                {
                    
                }
            });
        }
    }
}
