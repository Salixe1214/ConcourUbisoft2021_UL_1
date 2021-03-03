using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arm
{
    [RequireComponent(typeof(Collider))]
    public class MagnetTrigger : MonoBehaviour
    {
        private List<Pickable> pickables;

        private void Start()
        {
            pickables = new List<Pickable>();
        }

        private void FixedUpdate()
        {
            pickables.Clear();
        }

        private void OnTriggerStay(Collider other)
        {
            Pickable pickable = other.gameObject.GetComponent<Pickable>();
            if (pickable)
            {
                pickables.Add(pickable);
            }
        }


        public List<Pickable> GetPickables()
        {
            return pickables;
        }
    }
}