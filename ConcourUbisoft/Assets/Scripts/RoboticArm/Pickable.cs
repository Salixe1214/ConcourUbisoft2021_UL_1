using Other;
using Photon.Pun;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Arm
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Outline))]
    [RequireComponent(typeof(AudioSource))]
    public class Pickable : MonoBehaviour, IPunObservable
    {
        [SerializeField] private PickableType type;


        public Color Color { get { return _renderer.material.color; } set { _renderer.material.color = value; } }

        private Renderer _renderer = null;

        [SerializeField] private float volumeMultiplier=0.3f;
        [SerializeField] private AudioClip magnetCollisionSound;
        [SerializeField] private bool hasBeenPickup = false;
        private AudioSource _audioSource;
        private Rigidbody _rigidbody;

        private Collider _collider;
        private Outline _outline;
        private bool hovered;
        public Rigidbody RB => _rigidbody;

        public bool HasBeenPickup { get => hasBeenPickup; set => hasBeenPickup = value; }

        private static int nextId = 0;
        public int Id { get; private set; }
        private bool _grabbed = false;

        private NetworkController _networkController = null;

        private PhotonView photonView = null;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
            photonView = GetComponent<PhotonView>();

            if (!photonView.IsMine)
            {
                GetComponent<TransportableByConveyor>().enabled = false;
            }

            _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _outline = GetComponent<Outline>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _outline.enabled = false;
            Id = nextId++;

            if (!photonView.IsMine)
            {
                _rigidbody.isKinematic = true;
            }
        }

        public bool Contains(Vector3 point)
        {
            return _collider.bounds.Contains(point);
        }

        private void Update()
        {
            if (_outline.enabled && !hovered)
                _outline.enabled = false;
            else if (hovered)
                _outline.enabled = true;
            hovered = false;
        }

        public void OnGrab()
        {
            if (photonView.IsMine)
            {
                _rigidbody.useGravity = false;
                _rigidbody.freezeRotation = true;
                hasBeenPickup = true;
                _grabbed = true;
                _audioSource.clip = magnetCollisionSound;
                _audioSource.volume = volumeMultiplier;
                _audioSource.Play();
            }
        }

        public void OnHover()
        {
            hovered = true;
        }

        public void OnRelease()
        {
            if (photonView.IsMine)
            {
                transform.SetParent(null);
                _grabbed = false;
                _rigidbody.useGravity = true;
                _rigidbody.freezeRotation = false;
            }
        }

        public Vector3 GetBottomPosition()
        {
            Bounds bounds = _collider.bounds;

            return bounds.center - new Vector3(0, bounds.extents.y, 0);
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Color.r);
                stream.SendNext(Color.g);
                stream.SendNext(Color.b);
                stream.SendNext(Color.a);
            }
            else
            {
                Color = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
            }
        }

        public new PickableType GetType()
        {
            return type;
        }
    }
}