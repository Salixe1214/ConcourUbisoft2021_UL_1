using System;
using UnityEngine;

namespace Arm
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Outline))]
    [RequireComponent(typeof(AudioSource))]
    public class Pickable : MonoBehaviour
    {
        [SerializeField] private float volumeMultiplier=0.3f;
        [SerializeField] private AudioClip magnetCollisionSound;
        private AudioSource audioSource;
        private Rigidbody rigidbody;

        private Collider collider;
        private Outline outline;
        private bool hovered;
        public Rigidbody RB => rigidbody;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            outline = GetComponent<Outline>();
            audioSource = GetComponent<AudioSource>();
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
            audioSource.clip = magnetCollisionSound;
            audioSource.volume = volumeMultiplier;
            audioSource.Play();
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