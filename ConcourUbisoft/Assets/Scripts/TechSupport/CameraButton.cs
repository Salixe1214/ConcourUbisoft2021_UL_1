using TMPro;
using UnityEngine;

namespace TechSupport
{
    public class CameraButton : MonoBehaviour
    {
        public TextMeshProUGUI text;

        private Camera _camera;

        public void SetCamera(Camera camera)
        {
            _camera = camera;
            text.text = camera.gameObject.name;
        }

        public void CameraOn()
        {
            CameraSwitch.Current.enabled = false;
            _camera.enabled = true;
            CameraSwitch.Current = _camera;
        }
    }
    
}