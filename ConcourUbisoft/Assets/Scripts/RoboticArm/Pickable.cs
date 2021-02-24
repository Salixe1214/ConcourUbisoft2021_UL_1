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
        [SerializeField] private bool hasBeenPickup = false;
        private AudioSource audioSource;
        private Rigidbody rigidbody;

        private Collider collider;
        private Outline outline;
        private bool hovered;
        public Rigidbody RB => rigidbody;

        public bool HasBeenPickup { get => hasBeenPickup; set => hasBeenPickup = value; }

        private static int nextId = 0;
        public int Id { get; private set; }

        private void Start()
        {
            
            Id = nextId++;
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

        private void FixedUpdate()
        {
            if (outline.enabled && !hovered)
                outline.enabled = false;
            else if (hovered)
                outline.enabled = true;
            hovered = false;
        }

        public void OnGrab()
        {
            rigidbody.useGravity = false;
            rigidbody.freezeRotation = true;
            hasBeenPickup = true;
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
            rigidbody.useGravity = true;
            rigidbody.freezeRotation = false;
            
        }
    }
}