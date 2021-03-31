using System.Linq;
using UnityEngine;

namespace Arm
{
	public class MagnetController : MonoBehaviour
	{
		[SerializeField] private float pullForce = 10f;
		[SerializeField] private Transform magnetPullPoint;
		[SerializeField] private GameController.Role _owner = GameController.Role.SecurityGuard;
		[SerializeField] private GameController.Role _outlineViewer = GameController.Role.Technician;
		[SerializeField] private MagnetSound _magnetSound = null;

		private MagnetTrigger _magnetTrigger;
		private Pickable _currentPickable = null;
		private bool _grabbed = false;
		private NetworkController _networkController = null;

		public bool MagnetActive { get; set; }

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
					else if (_grabbed)
					{
						ReleasePickable();
					}
				}

				_magnetSound.IsOn = MagnetActive;
			}

			if (_outlineViewer == localRole)
			{
				if (_currentPickable != null)
				{
					_currentPickable.OnHover();
				}
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if (_owner == _networkController.GetLocalRole())
			{
				if (_currentPickable && !_grabbed && MagnetActive)
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

			_grabbed = true;
		}

		private void ReleasePickable()
		{
			_currentPickable.OnRelease();
			_currentPickable = null;
			_grabbed = false;
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