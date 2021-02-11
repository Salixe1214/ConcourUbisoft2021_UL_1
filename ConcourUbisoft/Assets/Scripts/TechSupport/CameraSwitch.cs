using System.Collections.Generic;
using UnityEngine;

namespace TechSupport
{
    public class CameraSwitch : MonoBehaviour
    {
        public List<Camera> cameras;
        public GameObject buttonPrefabs;
        
        public static Camera Current;
        
        private Vector3 _step = new Vector3(0f, 40f);

        void Start()
        {
            Current = GetFirstCameraActive();
            CreateButtons();
        }

        private Camera GetFirstCameraActive()
        {
            return cameras.Find(cam => cam.enabled);
        }

        private void CreateButtons()
        {
            Vector3 position = buttonPrefabs.transform.position;

            foreach (Camera camera in cameras)
            {
                CreateButton(camera, position);
                
                position += _step;
            }

        }

        private void CreateButton(Camera camera, Vector3 position)
        {
            GameObject button = Instantiate(buttonPrefabs, position, Quaternion.identity, gameObject.transform);
            if (button != null)
            {
                button.GetComponent<CameraButton>()?.SetCamera(camera);
            }
        }
    }
}
