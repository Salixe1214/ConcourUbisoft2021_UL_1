
using UnityEngine;

namespace Arm
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Outline))]
    public class Pickable : MonoBehaviour
    {
        private Rigidbody rigidbody;
        private Collider collider;
        private Outline outline;
        private bool hovered;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }

        public bool Contains(Vector3 point)
        {
            return collider.bounds.Contains(point);
        }

        private void Update()
        {
            if (outline.enabled && !hovered)
                outline.enabled = false;
            else if (hovered)
                outline.enabled = true;
            hovered = false;
        }

        public void OnGrab()
        {
            rigidbody.isKinematic = true;
        }

        public void OnHover()
        {
            hovered = true;
        }

        public void OnRelease()
        {
            rigidbody.isKinematic = false;
        }
    }
}