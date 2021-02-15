using UnityEngine;

namespace TechSupport.Surveillance
{
    [RequireComponent(typeof(Camera))]
    public class SurveillanceCamera : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
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

        public Vector3 GetCenter()
        {
            Rect rect = GetPrintedRect();
            return new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2);
        }

        public Vector3 TranslatePosition(Vector3 from)
        {
            return from * GetPrintedRect().size;
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
    }
}
