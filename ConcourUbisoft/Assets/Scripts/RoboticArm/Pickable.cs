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
        [SerializeField] private float volumeMultiplier = 0.3f;
        [SerializeField] private AudioClip magnetCollisionSound;
        [SerializeField] private bool hasBeenPickup = false;

        public Color Color { get { return _renderer.material.color; } set { _renderer.material.color = value; } }
        public Rigidbody Rigidbody { get; private set; }
        public bool HasBeenPickup { get => hasBeenPickup; set => hasBeenPickup = value; }

        private Renderer _renderer = null;
        private AudioSource _audioSource;
        private Collider _collider;
        private Outline _outline;
        private NetworkController _networkController = null;
        private PhotonView _photonView = null;
        private TransportableByConveyor _transportableByConveyor = null;

        private bool hovered;
        private bool _grabbed = false;

        private Vector3 _newPosition = new Vector3();

        private void Awake()
        {
            _newPosition = transform.position;
            _renderer = GetComponent<Renderer>();
            _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
            _transportableByConveyor = GetComponent<TransportableByConveyor>();
            _photonView = GetComponent<PhotonView>();

            if (!_photonView.IsMine)
            {
                GetComponent<TransportableByConveyor>().enabled = false;
            }

            _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
            Rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _outline = GetComponent<Outline>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _outline.enabled = false;

            if (!_photonView.IsMine)
            {
                Rigidbody.isKinematic = true;
            }
        }

        public bool Contains(Vector3 point)
        {
            return _collider.bounds.Contains(point);
        }

        private void Update()
        {
            if (!_photonView.IsMine)
            {
                transform.position = Vector3.MoveTowards(transform.position, _newPosition, (_grabbed ? 4 : _transportableByConveyor.IsOnConveyor() ? 3 : 10) * Time.deltaTime);
            }

            if (_outline.enabled && !hovered)
                _outline.enabled = false;
            else if (hovered)
                _outline.enabled = true;
            hovered = false;
        }

        public void OnGrab()
        {
            if (_photonView.IsMine)
            {
                Rigidbody.useGravity = false;
                Rigidbody.freezeRotation = true;
                hasBeenPickup = true;
                _grabbed = true;
                _photonView.RPC("PlayMagnetSound", RpcTarget.All);
            }
        }

        [PunRPC]
        public void PlayMagnetSound()
        {
            _audioSource.clip = magnetCollisionSound;
            _audioSource.volume = volumeMultiplier;
            _audioSource.Play();
        }

        public void OnHover()
        {
            hovered = true;
        }

        public void OnRelease()
        {
            if (_photonView.IsMine)
            {
                transform.SetParent(null);
                _grabbed = false;
                Rigidbody.useGravity = true;
                Rigidbody.freezeRotation = false;
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
                stream.SendNext(_grabbed);
                stream.SendNext(Color.r);
                stream.SendNext(Color.g);
                stream.SendNext(Color.b);
                stream.SendNext(Color.a);
                stream.SendNext(transform.position.x);
                stream.SendNext(transform.position.y);
                stream.SendNext(transform.position.z);
                stream.SendNext(transform.rotation.x);
                stream.SendNext(transform.rotation.y);
                stream.SendNext(transform.rotation.z);
                stream.SendNext(transform.rotation.w);
            }
            else
            {
                _grabbed = (bool)stream.ReceiveNext();
                Color = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
                _newPosition = new Vector3((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
                transform.rotation = new Quaternion((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
            }
        }

        public new PickableType GetType()
        {
            return type;
        }
    }
}