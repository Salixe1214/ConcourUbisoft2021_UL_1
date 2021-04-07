using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace Arm
{
	public class MagnetController : MonoBehaviour
	{
		public delegate void OnMagnetActiveChangeHandler();

		public event OnMagnetActiveChangeHandler OnMagnetActiveChange;

		public delegate void OnGrabStateChangeHandler();

		public event OnGrabStateChangeHandler OnGrabStateChange;
		[SerializeField] private float pullForce = 10f;
		[SerializeField] private Transform magnetPullPoint;
		[SerializeField] private GameController.Role _owner = GameController.Role.SecurityGuard;
		[SerializeField] private GameController.Role _outlineViewer = GameController.Role.Technician;
		[SerializeField] private MagnetSound _magnetSound = null;


		private MagnetTrigger _magnetTrigger;
		private Pickable _currentPickable = null;
		private bool _grabbed = false;
		private NetworkController _networkController = null;
		private bool _magnetActive = false;

		public bool Grabbed
		{
			get => _grabbed;
			set
			{
				_grabbed = value;
				OnGrabStateChange?.Invoke();
			}
		}

		public bool MagnetActive
		{
			get => _magnetActive;
			set
			{
				_magnetActive = value;
				OnMagnetActiveChange?.Invoke();
			}
		}

		private void Awake()
		{
			_magnetTrigger = GetComponentInChildren<MagnetTrigger>();
			_networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
		}

		private void Update()
		{
			transform.rotation = Quaternion.Euler(180, 0, 0);
			GameController.Role localRole = _networkController.GetLocalRole();
			UpdateCurrentPickable();
			if (_owner == localRole)
			{
				if (_currentPickable != null)
				{
					if (MagnetActive)
					{
						MovePickableToMagnet();
					}
					else if (Grabbed)
					{
						ReleasePickable();
					}
				}

				_magnetSound.IsOn = MagnetActive && !_grabbed;
			}

			if (_outlineViewer == localRole)
			{
				if (_currentPickable != null)
				{
					_currentPickable.OnHover();
				}
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			if (_owner == _networkController.GetLocalRole())
			{
				if (_currentPickable && !Grabbed && MagnetActive)
				{
					GrabPickable();
				}
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if (_owner == _networkController.GetLocalRole())
			{
				if (_currentPickable && !Grabbed && MagnetActive)
				{
					GrabPickable();
				}
			}
		}

		private void GrabPickable()
		{
			_currentPickable.OnGrab();
			_currentPickable.Rigidbody.velocity = Vector3.zero;
			_currentPickable.transform.parent = this.transform;
			Grabbed = true;
		}

		private void ReleasePickable()
		{
			_currentPickable.OnRelease();
			_currentPickable = null;
			Grabbed = false;
		}

		private void MovePickableToMagnet()
		{
			Vector3 directionToMagnet = (magnetPullPoint.position - _currentPickable.transform.position).normalized;

			_currentPickable.Rigidbody.velocity = pullForce * directionToMagnet;
		}

		private void UpdateCurrentPickable()
		{
			_currentPickable = _magnetTrigger.GetPickables().OrderBy(x => Vector3.Distance(x.GetBottomPosition(), magnetPullPoint.position)).FirstOrDefault();
		}
	}
}