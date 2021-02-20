using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arm
{
    public class MagnetController : MonoBehaviour
    {
        [SerializeField] private float pullForce = 0.1f;
        [SerializeField] private Controllable controllable;
        [SerializeField] private Transform magnetPullPoint;
        [SerializeField] private MagnetTrigger magnetTrigger;
        [SerializeField] private Transform magnetRotationRoot;
        private Pickable currentPickable = null;
        private bool grabbed = false;
        private bool magnetActive = false;
        public bool IsMagnetActive => magnetActive;

        private void Start()
        {
            controllable.OnControlStateChange += OnControlStateChange;
        }

        void OnControlStateChange(bool controlled)
        {
            if (!controlled)
            {
                TurnMagnetOff();
            }
        }

        private void Update()
        {
            magnetRotationRoot.rotation = Quaternion.Euler(190, 0, 0);
        }

        private void FixedUpdate()
        {
            if (controllable.IsControlled)
            {
                if (!grabbed && !currentPickable)
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

                if (!grabbed && 
                    magnetActive && 
                    currentPickable &&
                    magnetTrigger.GetPickables().Contains(currentPickable))
                    MovePickableToMagnet();
            }
        }

        private void MovePickableToMagnet()
        {
            currentPickable.RB.velocity += Time.fixedTime * pullForce *
                                           (magnetPullPoint.position - currentPickable.transform.position).normalized;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (magnetActive)
            {
                currentPickable = other.gameObject.GetComponent<Pickable>();
                if (currentPickable)
                {
                    currentPickable.OnGrab();
                    currentPickable.RB.velocity = Vector3.zero;
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
            currentPickable = null;
            List<Pickable> pickables = magnetTrigger.GetPickables();
            if (pickables.Count != 0)
            {
                float minDist = float.MaxValue;
                Pickable pickable = null;
                for (int i = pickables.Count - 1; i >= 0; i--)
                {
                    pickable = pickables[i];
                    if (pickable != null)
                    {
                        float dist = Vector3.Distance(pickable.transform.position, magnetPullPoint.position);
                        if (minDist > dist)
                        {
                            currentPickable = pickable;
                        }
                    }
                    else
                    {
                        pickables.RemoveAt(i);
                    }
                }
            }
        }
    }
}