﻿using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

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

        public float ControlSpeed => controlSpeed;
        public Transform Head => armIKSolver.transform;
        public Transform ArmTarget { get; private set; } = null;
        private float maxRange;

        private NetworkController _networkController = null;
        private PhotonView _photonView = null;

        private void Awake()
        {
            _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
            _photonView = GetComponentInParent<PhotonView>();
            if (_networkController.GetLocalRole() == _owner)
            {
                _photonView.RequestOwnership();
            }
        }

        private void Start()
        {
            maxRange = armIKSolver.TotalLength - 0.01f;
            ArmTarget = armIKSolver.Target;
        }

        void Update()
        {
            ClampTarget();
            FaceTarget();
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
        }
        public void Translate(Vector3 translate)
        {
            Vector3 translation = Vector3.ClampMagnitude(translate, controlSpeed);

            if (translate.magnitude >= float.Epsilon)
            {
                ArmTarget.transform.Translate(Time.deltaTime * controlSpeed * translation);
                _armSound.Volume = 0.3f;//translate.magnitude / (ControlSpeed * Time.deltaTime);
            }
            else
            {
                _armSound.Volume = 0;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(ArmTarget.position.x);
                stream.SendNext(ArmTarget.position.y);
                stream.SendNext(ArmTarget.position.z);
            }
            else
            {
                Vector3 newPostion = new Vector3((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
                ArmTarget.position = Vector3.MoveTowards(ArmTarget.position, newPostion, Time.deltaTime * controlSpeed);
            }
        }
    }
}