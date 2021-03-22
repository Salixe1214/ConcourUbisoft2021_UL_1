using System.Collections.Generic;
using TechSupport.Controller;
using UnityEngine;

namespace TechSupport.Surveillance
{
    [RequireComponent(typeof(Camera))]
    public class SurveillanceCamera : MonoBehaviour
    {
        private Camera _camera;

        [SerializeField] private List<ControllableOutline> controllable = new List<ControllableOutline>();

        public void Init()
        {
            _camera = GetComponent<Camera>();
        }

        public Rect GetPrintedRect()
        {
            Rect rect = _camera.rect;

            return new Rect(
                    rect.x * Screen.width,
                    rect.y * Screen.height,
                (rect.x + rect.width) * Screen.width,
                (rect.y + rect.height) * Screen.height);
        }
        
        public bool Contains(Vector3 point)
        {
            Rect rect = GetPrintedRect();
            return (point.x > rect.x && point.x < rect.width) &&
                   (point.y > rect.y && point.y < rect.height);
        }

        public Camera GetCamera()
        {
            return _camera;
        }

        public void Enable(bool enabledCamera)
        {
            _camera.enabled = enabledCamera;
        }

        public void EnableController(bool enableController)
        {
            controllable.ForEach(outline => outline.Enable(enableController, _camera));
        }
    }
}
