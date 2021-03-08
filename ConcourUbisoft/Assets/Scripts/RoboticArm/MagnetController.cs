using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Arm
{
    public class MagnetController : Serializable
    {
        [Serializable]
        public class MagnetControllerDTO
        {

        }

        [SerializeField] private float pullForce = 10f;
        [SerializeField] private Transform magnetPullPoint;
        [SerializeField] private MagnetTrigger magnetTrigger;
        [SerializeField] private Pickable currentPickable = null;

        private bool MagnetActive { get; set; }

        private void Update()
        {
            UpdateCurrentPickable();

            if (currentPickable != null)
            {
                currentPickable.OnHover();

                if (MagnetActive)
                {
                    MovePickableToMagnet();
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            collision.gameObject.GetComponent<Pickable>();
        }

        public override void Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize()
        {
            throw new NotImplementedException();
        }

        public override void Smooth(byte[] oldData, byte[] newData, float lag, double _lastTime, double _currentTime)
        {
            throw new NotImplementedException();
        }

        private void MovePickableToMagnet()
        {
            Vector3 directionToMagnet = (magnetPullPoint.position - currentPickable.transform.position).normalized;

            currentPickable.RB.velocity = pullForce * directionToMagnet;
        }

        private void UpdateCurrentPickable()
        {
            currentPickable = magnetTrigger.GetPickables().OrderBy(x => Vector3.Distance(x.transform.position, magnetPullPoint.position)).FirstOrDefault();
        }

        //[SerializeField] private float pullForce = 10f;
        //[SerializeField] private Controllable controllable;
        //[SerializeField] private Transform magnetPullPoint;
        //[SerializeField] private MagnetTrigger magnetTrigger;
        //[SerializeField] private Transform magnetRotationRoot;
        //[SerializeField] private Pickable currentPickable = null;
        //[SerializeField] private bool grabbed = false;
        //[SerializeField] private bool magnetActive = false;
        //[SerializeField] private bool _hasControlPanel = false;

        //public bool IsMagnetActive => magnetActive;

        //public bool MagnetActive
        //{
        //    get => magnetActive;
        //    set
        //    {
        //        if (value)
        //            magnetActive = value;
        //        else
        //        {
        //            magnetActive = false;
        //            grabbed = false;
        //            if (currentPickable)
        //            {
        //                Release();
        //            }
        //        }
        //    }
        //}

        //private NetworkSync _networkSync = null;
        //private NetworkController _networkController = null;

        //private void Start()
        //{
        //    controllable.OnControlStateChange += OnControlStateChange;
        //    _networkSync = GetComponent<NetworkSync>();
        //    GameObject networkControllerObject = GameObject.FindGameObjectWithTag("NetworkController");
        //    if (networkControllerObject != null)
        //        _networkController = networkControllerObject.GetComponent<NetworkController>();
        //}

        //void OnControlStateChange(bool controlled)
        //{
        //    if (!controlled)
        //    {
        //        MagnetActive = false;
        //    }
        //}

        //private void Update()
        //{
        //    magnetRotationRoot.rotation = Quaternion.Euler(180, 0, 0);
        //}

        //private void FixedUpdate()
        //{
        //    if (controllable.IsControlled ||
        //        (_networkController != null && _networkSync.Owner != _networkController.GetLocalRole()) || _hasControlPanel)
        //    {
        //        if (!grabbed && !currentPickable && _networkSync.Owner == _networkController.GetLocalRole())
        //        {
        //            UpdateCurrentPickable();
        //        }

        //        if (currentPickable)
        //        {
        //            currentPickable.OnHover();
        //            if (grabbed)
        //            {
        //                currentPickable.RB.velocity = Vector3.zero;
        //            }
        //        }

        //        if (_networkSync.Owner == _networkController.GetLocalRole() && !_hasControlPanel)
        //        {
        //            MagnetActive = (Input.GetButton("Grab") ||
        //                            Input.GetButton("GrabControllerXBO") ||
        //                            Input.GetButton("GrabControllerPS"));
        //        }

        //        if (!grabbed &&
        //            magnetActive &&
        //            currentPickable &&
        //            magnetTrigger.GetPickables().Contains(currentPickable))
        //            MovePickableToMagnet();
        //    }
        //}



        //private void OnCollisionStay(Collision other)
        //{
        //    if (magnetActive && grabbed != true)
        //    {
        //        currentPickable = other.gameObject.GetComponent<Pickable>();
        //        if (currentPickable)
        //        {
        //            currentPickable.OnGrab();
        //            currentPickable.RB.velocity = Vector3.zero;
        //            currentPickable.transform.parent = this.transform;
        //            grabbed = true;
        //        }
        //    }
        //}

        //private void Release()
        //{
        //    currentPickable.transform.SetParent(null);
        //    currentPickable.OnRelease();
        //    currentPickable = null;
        //}



        //public override byte[] Serialize()
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
        //        {
        //            if (currentPickable != null)
        //            {
        //                binaryWriter.Write((Int32) currentPickable.Id);
        //                binaryWriter.Write(currentPickable.transform.position.x);
        //                binaryWriter.Write(currentPickable.transform.position.y);
        //                binaryWriter.Write(currentPickable.transform.position.z);
        //                binaryWriter.Write(currentPickable.transform.rotation.x);
        //                binaryWriter.Write(currentPickable.transform.rotation.y);
        //                binaryWriter.Write(currentPickable.transform.rotation.z);
        //                binaryWriter.Write(currentPickable.transform.rotation.w);
        //            }
        //            else
        //            {
        //                binaryWriter.Write((Int32) (-1));
        //                binaryWriter.Write((float) (0.0f));
        //                binaryWriter.Write((float) (0.0f));
        //                binaryWriter.Write((float) (0.0f));
        //                binaryWriter.Write((float) (0.0f));
        //                binaryWriter.Write((float) (0.0f));
        //                binaryWriter.Write((float) (0.0f));
        //                binaryWriter.Write((float) (0.0f));
        //            }

        //            binaryWriter.Write((bool) magnetActive);
        //            binaryWriter.Write((bool) grabbed);
        //        }

        //        return memoryStream.ToArray();
        //    }
        //}

        //public override void Deserialize(byte[] data)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream(data))
        //    {
        //        using (BinaryReader binaryReader = new BinaryReader(memoryStream))
        //        {
        //            int pickableId = binaryReader.ReadInt32();
        //            Vector3 newPosition = new Vector3(
        //                binaryReader.ReadSingle(),
        //                binaryReader.ReadSingle(),
        //                binaryReader.ReadSingle());

        //            Quaternion quaternion = new Quaternion(binaryReader.ReadSingle(), binaryReader.ReadSingle(),
        //                binaryReader.ReadSingle(), binaryReader.ReadSingle());

        //            bool newMagnetActive = binaryReader.ReadBoolean();
        //            bool newGrabbed = binaryReader.ReadBoolean();
        //            //todo fix performance here
        //            Pickable pickable = GameObject.FindObjectsOfType<Pickable>().Where(x => x.Id == pickableId)
        //                .FirstOrDefault();
        //            if (pickableId == -1 && currentPickable != null)
        //            {
        //                Release();
        //            }

        //            if (pickable != null && currentPickable != null && currentPickable != pickable)
        //            {
        //                Release();
        //            }

        //            if (pickable != null)
        //            {
        //                currentPickable = pickable;
        //                if (grabbed == false)
        //                {
        //                    currentPickable.transform.position = newPosition;
        //                    currentPickable.transform.rotation = quaternion;
        //                }

        //                if (newGrabbed == true && grabbed == false)
        //                {
        //                    currentPickable.OnGrab();
        //                    currentPickable.RB.velocity = Vector3.zero;
        //                    currentPickable.transform.parent = this.transform;
        //                    grabbed = true;
        //                }
        //            }

        //            grabbed = newGrabbed;
        //            MagnetActive = newMagnetActive;
        //        }
        //    }
        //}

        //public override void Smooth(byte[] oldData, byte[] newData, float lag, double _lastTime, double _currentTime)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream(newData))
        //    {
        //        using (BinaryReader binaryReaderNew = new BinaryReader(memoryStream))
        //        {
        //            int pickableId = binaryReaderNew.ReadInt32();
        //            if (pickableId == -1)
        //            {
        //                currentPickable = null;
        //            }
        //            else
        //            {
        //                Pickable pickable = GameObject.FindObjectsOfType<Pickable>().Where(x => x.Id == pickableId)
        //                    .FirstOrDefault();
        //                if (pickable != null)
        //                {
        //                    Debug.Log("Found Pickable");
        //                    currentPickable = pickable;

        //                    Vector3 newPosition = new Vector3(
        //                        binaryReaderNew.ReadSingle(),
        //                        binaryReaderNew.ReadSingle(),
        //                        binaryReaderNew.ReadSingle());

        //                    Quaternion quaternion = new Quaternion(binaryReaderNew.ReadSingle(),
        //                        binaryReaderNew.ReadSingle(), binaryReaderNew.ReadSingle(),
        //                        binaryReaderNew.ReadSingle());
        //                    //currentPickable.transform.position = Vector3.MoveTowards(currentPickable.transform.position, newPosition, Time.deltaTime * 3);
        //                }
        //            }
        //        }
        //    }
        //}

        //public void ToggleMagnet()
        //{
        //    MagnetActive = !MagnetActive;
        //}
    }
}