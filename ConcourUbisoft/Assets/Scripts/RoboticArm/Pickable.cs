using System;
using TreeEditor;
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
            transform.SetParent(null);
            rigidbody.useGravity = true;
            rigidbody.freezeRotation = false;
        }

        public Vector3 GetBottomPosition()
        {
            Bounds bounds = collider.bounds;

            return bounds.center - new Vector3(0, bounds.extents.y, 0);
        }
    }
}