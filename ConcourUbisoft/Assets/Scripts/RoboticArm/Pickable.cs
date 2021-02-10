using System;
using UnityEngine;
using UnityEngine.Animations;

namespace Arm
{
    public class Pickable : MonoBehaviour
    {
        private Rigidbody rigidbody;
        private Collider collider;

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