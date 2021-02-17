using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arm
{
    public class MagnetController : MonoBehaviour
    {
        [SerializeField] private float pullForce = 1;
        [SerializeField] private Controllable controllable;
        [SerializeField] private float magnetRotation = 180;
        [SerializeField] private MagnetTrigger magnetTrigger;
        private Pickable currentPickable = null;
        private Vector3 currentPickableHitPosition;
        private bool grabbed = false;
        private bool magnetActive = false;
        public bool IsMagnetActive => magnetActive;
        private List<Pickable> pickables;


        private void Update()
        {
            transform.Rotate(new Vector3(0, 0, 1), magnetRotation - transform.eulerAngles.z);


            if (controllable.IsControlled)
            {
                if (!grabbed)
                {
                    UpdateCurrentPickable();
                }

                if (currentPickable)
                {
                    currentPickable.OnHover();
                }

                if (Input.GetButton("Grab") ||
                    Input.GetButton("GrabControllerXBO") ||
                    Input.GetButton("GrabControllerPS"))
                {
                    magnetActive = true;
                }
                else
                {
                    TurnMagnetOff();
                }
            }
            else
            {
                TurnMagnetOff();
            }
        }

        private void FixedUpdate()
        {
            if (controllable.IsControlled)
            {
                if (!grabbed && magnetActive && currentPickable)
                {
                    MovePickableToMagnet();
                }
            }
        }

        private void MovePickableToMagnet()
        {
            currentPickable.RB.velocity += Time.fixedTime * pullForce *
                                           (transform.position - currentPickable.transform.position).normalized;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (magnetActive)
            {
                currentPickable = other.gameObject.GetComponent<Pickable>();
                if (currentPickable)
                {
                    currentPickable.OnGrab();
                    currentPickable.transform.parent = this.transform;
                    grabbed = true;
                }
            }
        }

        private void TurnMagnetOff()
        {
            magnetActive = false;
            grabbed = false;
            if (currentPickable)
            {
                Release();
            }
        }

        private void Release()
        {
            currentPickable.transform.SetParent(null);
            currentPickable.OnRelease();
            currentPickable = null;
        }

        private void UpdateCurrentPickable()
        {
            List<Pickable> pickables = magnetTrigger.GetPickables();
            if (pickables.Count != 0)
            {
                float minDist = float.MaxValue;
                foreach (var pickable in pickables)
                {
                    float dist = Vector3.Distance(pickable.transform.position, transform.position);
                    if (minDist > dist)
                    {
                        currentPickable = pickable;
                    }
                }
            }
            else
            {
                currentPickable = null;
            }
        }
    }
}