using UnityEngine;

namespace TechSupport
{
    public class CameraRotate : MonoBehaviour
    {
        private readonly float _hSpeed = 2.0f;
        private readonly float _vSpeed = 2.0f;

        private float _horizon = 0f;
        private float _vertical = 0f;
        
        void Update()
        {
            _horizon += _hSpeed * Input.GetAxis("Mouse X");
            _vertical -= _vSpeed * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(_vertical, _horizon, 0.0f);
        }
    }
}
