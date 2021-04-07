using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Arm
{
	public class ArmController : MonoBehaviour, IPunObservable
	{
		[SerializeField] private float minRange = 1.5f;
		[SerializeField] float controlSpeed = 3f;
		[SerializeField] private IKSolver armIKSolver;
		[SerializeField] private Transform armRotationRoot;
		[SerializeField] private GameController.Role _owner = GameController.Role.None;
		[SerializeField] private ArmSound _armSound = null;
		[SerializeField] private Bounds boundingBox;
        [SerializeField] private ParticleSystem[] _particleSystems = null;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip electricityContinueSound;

		public float ControlSpeed => controlSpeed;
		public Transform Head => armIKSolver.transform;
		public Transform ArmTarget { get; private set; } = null;
		private float maxRange;

		private NetworkController _networkController = null;
		private PhotonView _photonView = null;

		private Vector3 newPosition = new Vector3();

		private Vector3 translation = new Vector3();
		private bool initialialized = false;
        private Matrix4x4 aligment = new Matrix4x4(new Vector4(1,0,0,0),new Vector4(0,1,0,0), new Vector4(0,0,1,0), new Vector4(0,0,0,0));

        private bool _inversedX = false;
        private bool _inversedZ = false;
        private bool _inversed => _inversedX || _inversedZ;

        private float _magn = 0.0f;

		private void Awake()
		{
            _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
			_photonView = GetComponentInParent<PhotonView>();
        }

		private void Start()
		{
            if (_networkController.GetLocalRole() == _owner && !_photonView.IsMine)
            {
                _photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            boundingBox.center = this.transform.position + boundingBox.center;
			initialialized = true;
			maxRange = armIKSolver.TotalLength - 0.01f;
			ArmTarget = armIKSolver.Target;
			newPosition = ArmTarget.position;
		}

		void Update()
		{
            ClampTarget();
			FaceTarget();

            if (_photonView.IsMine)
			{
				if (translation.magnitude >= float.Epsilon)
				{
					ArmTarget.transform.Translate(Time.deltaTime * aligment.MultiplyVector(translation.normalized) * controlSpeed);
					_armSound.Volume = _magn;
					_magn += Time.deltaTime;
				}
				else
				{
					_magn = 0.0f;
					_armSound.Volume = 0;
				}

				translation = Vector3.zero;
			}
            else
            {
                ArmTarget.position = Vector3.MoveTowards(ArmTarget.position, newPosition, Time.deltaTime * controlSpeed);
            }
		}

		private void FaceTarget()
		{
			Vector3 direction = ArmTarget.position - transform.position;
			direction.y = 0;
			direction.Normalize();
			float rotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			armRotationRoot.rotation = Quaternion.Euler(0f, rotationY + 180, 0f);
		}

		private void ClampTarget()
		{
			float distanceToTarget = Vector3.Distance(transform.position, ArmTarget.position);
			if (distanceToTarget > maxRange)
			{
				Vector3 dirToTarget = (ArmTarget.position - transform.position).normalized;
				ArmTarget.position = new Vector3(
					transform.position.x + dirToTarget.x * maxRange,
					ArmTarget.position.y,
					transform.position.z + dirToTarget.z * maxRange);
			}

			float flatDistanceToTarget =
				Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
					new Vector3(armIKSolver.Target.position.x, 0, armIKSolver.Target.position.z));
			if (flatDistanceToTarget < minRange)
			{
				Vector3 dirToTarget = (ArmTarget.position - transform.position);
				dirToTarget.y = 0;
				dirToTarget.Normalize();
				ArmTarget.position = new Vector3(
					transform.position.x + dirToTarget.x * minRange,
					ArmTarget.position.y,
					transform.position.z + dirToTarget.z * minRange);
			}

			if (!boundingBox.Contains(ArmTarget.position))
			{
				ArmTarget.position = boundingBox.ClosestPoint(ArmTarget.position);
			}
		}

        public bool IsInversedX()
        {
            return aligment.GetRow(0).x == -1;
        }

        public bool IsInversedZ()
        {
            return aligment.GetRow(2).z == -1;
        }

        public void InverseX()
        {
            aligment.SetRow(0, aligment.GetRow(0) * -1);
            _inversedX = !_inversedX;
            foreach (ParticleSystem particleSystem in _particleSystems)
            {
                if (_inversed)
                {
                    if (!particleSystem.isPlaying)
                    {
                        particleSystem.Play();
                        audioSource.loop = true;
                        audioSource.clip = electricityContinueSound;
                        audioSource.volume = 0.25f;
                        audioSource.Play();
                    }
                }
                else
                {
                    if (!particleSystem.isStopped)
                    {
                        particleSystem.Stop();
                        audioSource.Stop();
                        audioSource.loop = false;
                    }
                }
            }
        }

        public void InverseZ()
        {
            aligment.SetRow(2, aligment.GetRow(2) * -1);
            _inversedZ = !_inversedZ;
            foreach (ParticleSystem particleSystem in _particleSystems)
            {
                if (_inversed)
                {
                    if(!particleSystem.isPlaying)
                    {
                        particleSystem.Play();
                        audioSource.loop = true;
                        audioSource.clip = electricityContinueSound;
                        audioSource.volume = 0.25f;
                        audioSource.Play();
                    }
                }
                else
                {
                    if (!particleSystem.isStopped)
                    {
                        particleSystem.Stop();
                        audioSource.Stop();
                        audioSource.loop = false;
                    }
                }
            }
        }

		public void Translate(Vector3 translate)
		{
			translation += translate;
		}

#if UNITY_EDITOR

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			if (!initialialized)
			{
				Gizmos.DrawWireCube(transform.position + boundingBox.center, boundingBox.size);
			}
			else
			{
				Gizmos.DrawWireCube(boundingBox.center, boundingBox.size);
			}
		}
#endif
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(ArmTarget.position.x);
				stream.SendNext(ArmTarget.position.y);
				stream.SendNext(ArmTarget.position.z);
			}
			else
			{
				newPosition = new Vector3((float) stream.ReceiveNext(), (float) stream.ReceiveNext(), (float) stream.ReceiveNext());
			}
		}
	}
}