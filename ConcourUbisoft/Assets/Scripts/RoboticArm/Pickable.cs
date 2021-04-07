using Other;
using Photon.Pun;
using System;
using System.Collections;
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
		[SerializeField] private AudioClip onHitSound;
		[SerializeField] private float _intensity = 0.0f;
		[SerializeField] private int _directionIntensity = 1;
		[SerializeField] private float _intensityGainSpeed = 0.2f;
		[SerializeField] private float _intensityUpperBound = 0.4f;
		[SerializeField] private float _intensityBottomBound = 0.0f;

		public Color Color
		{
			get { return _renderer.material.color; }
			set { _renderer.material.color = value; }
		}

		public Rigidbody Rigidbody { get; private set; }
		public bool Consumed { get; set; } = false;
		public FurnaceController Furnace { get; set; } = null;

		public bool IsGrabbed
		{
			get { return _grabbed; }
		}

		private Renderer _renderer = null;
		private AudioSource _audioSource;
		private Collider _collider;
		private Outline _outline;
		private NetworkController _networkController = null;
		private PhotonView _photonView = null;
		private TransportableByConveyor _transportableByConveyor = null;
		private GameController _gameController = null;
		private float _xIntensity = 0;

		private bool hovered;
		private bool _grabbed = false;
		private float _conveyorSpeed = 0.0f;
		public GameController.Role _emissionVisibleBy = GameController.Role.None;
		private Vector3 _newPosition = new Vector3();
		public bool _isRightColor = false;

		private bool grabbedAtleastOnce = false;

		private void Awake()
		{
			_newPosition = transform.position;
			_renderer = GetComponent<Renderer>();
			_networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
			_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			_transportableByConveyor = GetComponent<TransportableByConveyor>();
			_photonView = GetComponent<PhotonView>();
			Rigidbody = GetComponent<Rigidbody>();
			_collider = GetComponent<Collider>();
			_outline = GetComponent<Outline>();
			_audioSource = GetComponent<AudioSource>();
		}

		private void Start()
		{
			_outline.enabled = false;
		}

		private void OnCollisionEnter(Collision other)
		{
			if (grabbedAtleastOnce && gameObject.activeInHierarchy && !other.gameObject.CompareTag("Magnet"))
			{
				Debug.LogWarning(other.gameObject.name);
                _photonView.RPC("PlayHitSoundNetwork", RpcTarget.All);
			}
		}

		[PunRPC]
		private void PlayHitSoundNetwork()
		{
			_audioSource.clip = onHitSound;
			_audioSource.volume = volumeMultiplier;
			_audioSource.Play();
		}

		public bool Contains(Vector3 point)
		{
			return _collider.bounds.Contains(point);
		}

		private void Update()
		{
			if (_gameController.ColorBlindMode && Furnace != null)
			{
				Color color = Furnace.GetNextColor();
				_isRightColor = Math.Abs(color.r - Color.r) < 0.1f && Math.Abs(color.g - Color.g) < 0.1f && Math.Abs(color.b - Color.b) < 0.1f;
			}

			if (_networkController.GetLocalRole() == _emissionVisibleBy && _isRightColor)
			{
				_renderer.material.SetColor("_EmissionColor", Color * _intensity);
				if (!_renderer.material.IsKeywordEnabled("_EMISSION"))
				{
					_renderer.material.EnableKeyword("_EMISSION");
				}

				_xIntensity = _xIntensity + _directionIntensity * Time.deltaTime * _intensityGainSpeed;

				_intensity = _xIntensity * _xIntensity;

				if (_intensity < _intensityBottomBound)
				{
					_xIntensity = 0;
					_intensity = _intensityBottomBound;
					_directionIntensity = 1;
				}

				if (_intensity > _intensityUpperBound)
				{
					_intensity = _intensityUpperBound;
					_xIntensity = Mathf.Sqrt(_intensity);
					_directionIntensity = -1;
				}
			}
			else
			{
				if (_renderer.material.IsKeywordEnabled("_EMISSION"))
				{
					_directionIntensity = 1;
					_intensity = _intensityBottomBound;
					_renderer.material.DisableKeyword("_EMISSION");
				}
			}

			if (!_photonView.IsMine && _conveyorSpeed == 0.0f)
			{
				if (Vector3.Distance(transform.position, _newPosition) > 3)
				{
					transform.position = _newPosition;
				}
				else
				{
					transform.position = Vector3.MoveTowards(transform.position, _newPosition, (_grabbed ? 4 : 10) * Time.deltaTime);
				}
			}

			if (_outline.enabled && !hovered)
				_outline.enabled = false;
			else if (hovered)
				_outline.enabled = true;
			hovered = false;
		}

		public void OnGrab()
		{
			grabbedAtleastOnce = true;
			if (_photonView.IsMine)
			{
				Rigidbody.useGravity = false;
				Rigidbody.freezeRotation = true;
				Rigidbody.isKinematic = true;
				_grabbed = true;
				_photonView.RPC("PlayMagnetSound", RpcTarget.All);
			}
		}

		public void SetEmissionVisibleBy(GameController.Role role)
		{
			_photonView.RPC("SetEmissionVisibleByRPC", RpcTarget.All, new object[] {(int) role} as object);
		}

		[PunRPC]
		private void SetEmissionVisibleByRPC(object[] parameters)
		{
			_emissionVisibleBy = (GameController.Role) parameters[0];
		}

		[PunRPC]
		private void PlayMagnetSound()
		{
			_audioSource.clip = magnetCollisionSound;
			_audioSource.volume = volumeMultiplier;
			_audioSource.Play();
		}

		public void SetActiveNetwork(bool active)
		{
			_photonView.RPC("SetActiveGameObject", RpcTarget.All, new object[] {(bool) active} as object);
		}

		[PunRPC]
		private void SetActiveGameObject(object[] parameters)
		{
			this.gameObject.SetActive((bool) parameters[0]);
			grabbedAtleastOnce = false;
		}

		public void SetConsumedNetwork(bool consumed)
		{
			_photonView.RPC("SetConsumed", RpcTarget.All, new object[] {(bool) consumed} as object);
		}

		[PunRPC]
		private void SetConsumed(object[] parameters)
		{
			Consumed = (bool) parameters[0];
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
				Rigidbody.isKinematic = false;
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
				stream.SendNext(_transportableByConveyor.ConveyorSpeed());
				stream.SendNext(_grabbed);
				stream.SendNext(Color.r);
				stream.SendNext(Color.g);
				stream.SendNext(Color.b);
				stream.SendNext(Color.a);
				stream.SendNext(_isRightColor);
				if (_transportableByConveyor.ConveyorSpeed() == 0.0f)
				{
					stream.SendNext(transform.position.x);
					stream.SendNext(transform.position.y);
					stream.SendNext(transform.position.z);
					stream.SendNext(transform.rotation.x);
					stream.SendNext(transform.rotation.y);
					stream.SendNext(transform.rotation.z);
					stream.SendNext(transform.rotation.w);
				}
			}
			else
			{
				_conveyorSpeed = (float) stream.ReceiveNext();
				_grabbed = (bool) stream.ReceiveNext();
				Color = new Color((float) stream.ReceiveNext(), (float) stream.ReceiveNext(), (float) stream.ReceiveNext(), (float) stream.ReceiveNext());
				_isRightColor = (bool) stream.ReceiveNext();
				if (_conveyorSpeed == 0.0f)
				{
					_newPosition = new Vector3((float) stream.ReceiveNext(), (float) stream.ReceiveNext(), (float) stream.ReceiveNext());
					transform.rotation = new Quaternion((float) stream.ReceiveNext(), (float) stream.ReceiveNext(), (float) stream.ReceiveNext(),
						(float) stream.ReceiveNext());
				}

				Rigidbody.isKinematic = _conveyorSpeed == 0.0f;
			}
		}

		public new PickableType GetType()
		{
			return type;
		}
	}
}