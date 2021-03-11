using System.Collections.Generic;
using UnityEngine;

namespace CameraScript
{
    [RequireComponent(typeof(Camera))]
    public class CameraLinkOverlays : MonoBehaviour
    {
        private Camera _main;
        [SerializeField] private List<Camera> overlays;

        private void Awake()
        {
            _main = GetComponent<Camera>();
        }

        void FixedUpdate()
        {
            overlays.ForEach(overlay =>
            {
                overlay.rect = _main.rect;
                overlay.enabled = _main.enabled;
            });
        }
    }
}
