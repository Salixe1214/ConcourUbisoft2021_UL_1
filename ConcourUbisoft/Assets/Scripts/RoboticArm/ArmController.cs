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
        [SerializeField] private float minRange = 1.5f;
        [SerializeField] float controlSpeed = 3f;
        [SerializeField] private Controllable controllable;
        [SerializeField] private IKSolver armIKSolver;
        [SerializeField] private Transform armRotationRoot;
        [SerializeField] private bool _hasControlPanel = false;

        private float maxRange;
        public float ControlSpeed => controlSpeed;

        public Transform ArmTarget { get; private set; } = null;

        #region controllerDeclaration

        private int _inputVerticalAxis;
        private int _inputHorizontalAxis;

        #endregion

        #region Network

        public override void Deserialize(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {

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

        public override void Smooth(byte[] oldData, byte[] newData, float lag, double lastTime, double currentTime)
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

                    ArmTarget.position = Vector3.MoveTowards(ArmTarget.position,newPosition, Time.deltaTime * controlSpeed);
                }
            }
        }

        #endregion

        private void Start()
        {
            maxRange = armIKSolver.TotalLength - 0.01f;
            ArmTarget = armIKSolver.Target;

            _inputVerticalAxis = 0;
            _inputHorizontalAxis = 0;
        }

        void Update()
        {
            if (controllable.IsControlled)
            {
                float inputV = !_hasControlPanel ? Input.GetAxis("Vertical") : _inputVerticalAxis;
                float inputH = !_hasControlPanel ? Input.GetAxis("Horizontal") : _inputHorizontalAxis;

                Vector3 translation =
                    Vector3.ClampMagnitude(new Vector3(inputV * -1, 0, inputH),
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
    
        public void OnHMove(int dir)
        {
            _inputHorizontalAxis = dir;
        }
    
        public void OnVMove(int dir)
        {
            _inputVerticalAxis = dir;
        }

        
    }
}