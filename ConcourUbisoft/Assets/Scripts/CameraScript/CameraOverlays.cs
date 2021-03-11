using System.Collections.Generic;
using UnityEngine;

namespace CameraScript
{
    [RequireComponent(typeof(Camera))]
    public class CameraOverlays : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField] private List<Camera> overlays;

        void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            overlays.ForEach(overlay =>
            {
                overlay.rect = _camera.rect;
                overlay.transform.position = transform.position;
            });
        }
    }
}
