using System;
using UnityEngine;

namespace Other
{
    public class RobotLight : MonoBehaviour
    {
        [SerializeField] private Color _lightColor = Color.red;
        private Material _lightMaterial;
        private Renderer _renderer;

        private void Start()
        {
            _lightMaterial = GetComponent<Material>();
            _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            
        }

        public void LightItUp()
        {
            _renderer.material.color = _lightColor * 15f;
        }
    }
}