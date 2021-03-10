using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Arm
{
    public class MagnetController : Serializable
    {
        [Serializable]
        public class MagnetControllerDTO
        {
            public int? PickableId { get; set; }
            public bool MagnetActive { get; set; }
            public bool Grabbed { get; set; }
            public float? PositionX { get; set; }
            public float? PositionY { get; set; }
            public float? PositionZ { get; set; }
            public float? RotationX { get; set; }
            public float? RotationY { get; set; }
            public float? RotationZ { get; set; }
            public float? RotationW { get; set; }
        }

        [SerializeField] private float pullForce = 10f;
        [SerializeField] private Transform magnetPullPoint;
        [SerializeField] private GameController.Role _owner = GameController.Role.SecurityGuard;

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
            if(_owner == _networkController.GetLocalRole())
            {
                UpdateCurrentPickable();

                if (_currentPickable != null)
                {
                    _currentPickable.OnHover();

                    if (MagnetActive)
                    {
                        MovePickableToMagnet();
                    }
                    else if (_grabbed)
                    {
                        ReleasePickable();
                    }
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (_currentPickable && !_grabbed && MagnetActive)
            {
                GrabPickable();
            }
        }

        private void GrabPickable()
        {
            _currentPickable.OnGrab();
            _currentPickable.RB.velocity = Vector3.zero;
            _currentPickable.transform.parent = this.transform;

            _grabbed = true;
        }

        private void ReleasePickable()
        {
            _currentPickable.OnRelease();
            _currentPickable = null;
            _grabbed = false;
        }
        
        public override void Deserialize(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var memStream = new MemoryStream())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memStream.Write(data, 0, data.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    MagnetControllerDTO serializedMagnetController = (MagnetControllerDTO)bf.Deserialize(memStream);

                    Pickable pickable = serializedMagnetController.PickableId != null ? GameObject.FindObjectsOfType<Pickable>().Where(x => x.Id == serializedMagnetController.PickableId) 
                        .FirstOrDefault() : null;
                    if (serializedMagnetController.PickableId == null && _currentPickable != null
                        || pickable != null && _currentPickable != null && _currentPickable != pickable)
                    {
                        ReleasePickable();
                    }

                    if(serializedMagnetController.PickableId == null)
                    {
                        _currentPickable = null;
                    }

                    if (pickable != null)
                    {
                        Vector3 newPosition = new Vector3(serializedMagnetController.PositionX.Value, serializedMagnetController.PositionY.Value, serializedMagnetController.PositionZ.Value);

                        Quaternion quaternion = new Quaternion(serializedMagnetController.RotationX.Value, serializedMagnetController.RotationY.Value, serializedMagnetController.RotationZ.Value, serializedMagnetController.RotationW.Value);
                        _currentPickable = pickable;

                        if (_grabbed == false && serializedMagnetController.MagnetActive)
                        {
                            _currentPickable.transform.position = newPosition;
                            _currentPickable.transform.rotation = quaternion;
                        }

                        if (serializedMagnetController.MagnetActive == false && MagnetActive == true && _grabbed == true)
                        {
                            ReleasePickable();
                        }

                        if (serializedMagnetController.Grabbed == true && _grabbed == false)
                        {
                            GrabPickable();
                        }
                    }

                    _grabbed = serializedMagnetController.Grabbed;
                    MagnetActive = serializedMagnetController.MagnetActive;
                }
            }
        }

        public override byte[] Serialize()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bf.Serialize(memoryStream, new MagnetControllerDTO() {
                    PickableId = _currentPickable?.Id,
                    Grabbed = _grabbed,
                    MagnetActive = MagnetActive,
                    PositionX = _currentPickable?.transform.position.x,
                    PositionY = _currentPickable?.transform.position.y,
                    PositionZ = _currentPickable?.transform.position.z,
                    RotationX = _currentPickable?.transform.rotation.x,
                    RotationY = _currentPickable?.transform.rotation.y,
                    RotationZ = _currentPickable?.transform.rotation.z,
                    RotationW = _currentPickable?.transform.rotation.w,
                });
                
                return memoryStream.ToArray();
            }
        }

        public override void Smooth(byte[] oldData, byte[] newData, float lag, double _lastTime, double _currentTime)
        {

        }

        private void MovePickableToMagnet()
        {
            Vector3 directionToMagnet = (magnetPullPoint.position - _currentPickable.transform.position).normalized;

            _currentPickable.RB.velocity = pullForce * directionToMagnet;
        }

        private void UpdateCurrentPickable()
        {
            _currentPickable = _magnetTrigger.GetPickables().OrderBy(x => Vector3.Distance(x.GetBottomPosition(), magnetPullPoint.position)).FirstOrDefault();

        }
    }
}