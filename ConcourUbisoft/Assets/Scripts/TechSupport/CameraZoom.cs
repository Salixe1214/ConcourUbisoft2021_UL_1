using UnityEngine;

namespace TechSupport
{
    public class CameraZoom : MonoBehaviour
    {
        private Camera _camera;

        private bool _zooming = false;
    
        private readonly float _hSpeed = 200.0f;
        private readonly float _vSpeed = 200.0f;

        private float _horizon = 0f;
        private float _vertical = 0f;

        private Quaternion _rotation;

        void Start()
        {
            _camera = GetComponent<Camera>();
            _rotation = transform.rotation;
        }

        private Rect GetCoordonateRect(Rect rect)
        {
            return new Rect(rect.x * Screen.width, rect.y * Screen.height, 
                (rect.x + rect.width) * Screen.width,
                (rect.y + rect.height) * Screen.height);
        }

        private bool Contains(Rect rect, Vector3 position)
        {
            return (position.x > rect.x && position.x < rect.width) &&
                   (position.y > rect.y && position.y < rect.height);
        }

        // TODO: Clean the code + improve zoom
        void Update()
        {
            bool mouseInCamera = Contains(GetCoordonateRect(_camera.rect), Input.mousePosition);
        
            if (Input.GetMouseButtonDown(1) && !_zooming && mouseInCamera)
            {
                _camera.fieldOfView -= 40;
                _zooming = true;
            } else if (Input.GetMouseButton(1) && _zooming && mouseInCamera)
            {
                _horizon += _hSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
                _vertical -= _vSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
            
                transform.eulerAngles = new Vector3(_vertical, _horizon, 0.0f);
            } else if (Input.GetMouseButtonUp(1) && _zooming)
            {
                transform.rotation = _rotation;
                _camera.fieldOfView += 40;
                _zooming = false;
            }
        }
    }
}
