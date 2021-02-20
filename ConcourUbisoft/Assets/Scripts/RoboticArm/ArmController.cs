using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

namespace Arm
{
    public class ArmController : Serializable
    {
        [SerializeField] private Controllable controllable;
        [SerializeField] private IKSolver armIKSolver;
        [SerializeField] private Transform armRotationRoot;
        [SerializeField] float controlSpeed = 3f;
        [SerializeField] private bool _extrapolation = false;
        [SerializeField] private bool _lerping = false;
        [SerializeField] private float _lerpingValue = 0.1f;
        [SerializeField] private bool _lagCompensation = false;
        private float maxRange;
        private float minRange;


        public float ControlSpeed => controlSpeed;

        public Transform ArmTarget { get; private set; } = null;

        public override void Deserialize(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    float positionX = binaryReader.ReadSingle();
                    float positionY = binaryReader.ReadSingle();
                    float positionZ = binaryReader.ReadSingle();
                }
            }
        }

        public override byte[] Serialize()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(ArmTarget.position.x);
                    binaryWriter.Write(ArmTarget.position.y);
                    binaryWriter.Write(ArmTarget.position.z);
                }

                return memoryStream.ToArray();
            }
        }

        public override void Smooth(byte[] oldData, byte[] newData, float lag, float lastTime, float currentTime)
        {
            using (MemoryStream memoryStreamOld = new MemoryStream(oldData),
                memoryStreamNew = new MemoryStream(newData))
            {
                using (BinaryReader binaryReaderOld = new BinaryReader(memoryStreamOld),
                    binaryReaderNew = new BinaryReader(memoryStreamNew))
                {
                    Vector3 newPosition = new Vector3(
                        binaryReaderNew.ReadSingle(),
                        binaryReaderNew.ReadSingle(),
                        binaryReaderNew.ReadSingle());

                    Vector3 oldPosition = new Vector3(
                        binaryReaderOld.ReadSingle(),
                        binaryReaderOld.ReadSingle(),
                        binaryReaderOld.ReadSingle());

                    Vector3 deltaPosition = newPosition - oldPosition;
                    Vector3 lagCompensation = _lagCompensation ? (deltaPosition / (currentTime - lastTime)) * lag : Vector3.zero;
                    Vector3 predicatedPosition = newPosition + deltaPosition * 2;
                    Vector3 valueToUse = (_extrapolation ? predicatedPosition : newPosition) + lagCompensation;

                    if (_lerping)
                    {
                        ArmTarget.position = Vector3.MoveTowards(ArmTarget.position, Vector3.Lerp(ArmTarget.position, valueToUse, _lerpingValue), Time.deltaTime * controlSpeed);
                    }
                    else
                    {
                        ArmTarget.position = Vector3.MoveTowards(ArmTarget.position, valueToUse, Time.deltaTime * controlSpeed);
                    }
                }
            }
        }

        private void Start()
        {
            minRange = armIKSolver.TotalLength / 6;
            maxRange = armIKSolver.TotalLength - minRange;
            ArmTarget = armIKSolver.Target;
        }

        void Update()
        {
            if (controllable.IsControlled)
            {
                Vector3 translation =
                    Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")),
                        controlSpeed);
                ArmTarget.transform.Translate(Time.deltaTime * controlSpeed * translation);
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

            Vector3 direction = ArmTarget.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            float rotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            armRotationRoot.rotation = Quaternion.Euler(0f, rotationY + 180, 0f);
        }
    }
}