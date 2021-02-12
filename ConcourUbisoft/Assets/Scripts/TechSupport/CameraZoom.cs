using UnityEngine;
using UnityEngine.UIElements;

namespace TechSupport
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private float zoom = 10;
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
        public Rect GetPrintedRect()
        {
            Rect rect = _camera.rect; // Avoid to make to much calls to the getter
            
            return new Rect(
                rect.x * Screen.width, 
                rect.y * Screen.height, 
                (rect.x + rect.width) * Screen.width,
                (rect.y + rect.height) * Screen.height);
        }

        public Vector3 GetCenter()
        {
            Rect rect = GetPrintedRect();
            return rect.position + rect.size / 2;
        }

        public Vector3 GetScreenCenter()
        {
            return new Vector3(Screen.width / 2, Screen.height / 2);
        }

        public Vector3 TranslatePosition(Vector3 from)
        {
            return from / GetPrintedRect().size;
        }

        // TODO: Clean the code + improve zoom
        void Update()
        {
            bool mouseInCamera = Contains(GetCoordonateRect(_camera.rect), Input.mousePosition);
        
            if (Input.GetMouseButtonDown(1) && !_zooming && mouseInCamera)
            {
                _zooming = true;
                _camera.fieldOfView -= zoom;
                //transform.Rotate(new Vector3(TranslatePosition(Input.mousePosition), 0));
                Vector3 test = ((Vector2)TranslatePosition(Input.mousePosition)) - _camera.rect.size;
                test.y = -test.y;
                //Debug.Log(test);
                transform.eulerAngles = test * zoom;
                //GetCenter() - TranslatePosition(Input.mousePosition);
            } else if (Input.GetMouseButtonUp(1) && _zooming)
            {
                transform.rotation = _rotation;
                _camera.fieldOfView += zoom;
                _zooming = false;
            }
        }
    }
}
