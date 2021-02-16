using UnityEngine;

namespace Arm
{
    public class MagnetController : MonoBehaviour
    {
        [SerializeField] private float pullForce = 1;
        [SerializeField] private Controllable controllable;
        [SerializeField] private float magnetRotation = 180;
        private Pickable currentPickable = null;
        private Vector3 currentPickableHitPosition;
        private bool grabbed = false;
        private bool magnetActive = false;
        public bool IsMagnetActive => magnetActive;

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
            RaycastHit hit;
            Vector3 direction = transform.up;

            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, 1 << 6))
            {
                currentPickable = hit.transform.GetComponent<Pickable>();
                if (currentPickable)
                {
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
                }
            }
            else
            {
                Debug.DrawRay(transform.position, direction * 100, Color.red);
            }
        }
    }
}