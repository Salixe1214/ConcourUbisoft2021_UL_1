using UnityEngine;

namespace Arm
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Pickable : MonoBehaviour
    {
        private new Rigidbody rigidbody;
        private new Collider collider;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
        }

        public bool Constains(Vector3 point)
        {
            return collider.bounds.Contains(point);
        }

        public void OnGrab()
        {
            rigidbody.isKinematic = true;
        }

        public void OnRelease()
        {
            rigidbody.isKinematic = false;
        }
    }
}