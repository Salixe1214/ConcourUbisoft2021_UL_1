using System;
using UnityEngine;

namespace Arm
{
    enum ControlScheme
    {
        WASD,
        ROTATION,
    }

    enum GrabState
    {
        NONE,
        MOVE_TO_TARGET,
        GRABBING,
        GRABBED,
        RESET_POSITION,
    }

    public class ArmController : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private IKSolver armIKSolver;
        [SerializeField] private ControlScheme scheme;
        [SerializeField] float speed = 3f;
        [SerializeField] private float grabSpeed = 3f;
        [SerializeField] private Transform targetRotationRoot;
        [SerializeField] private Transform armRotationRoot;
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float headRotation = 180;
        private float maxRange;
        private float minRange;
        private GrabState grabState = GrabState.NONE;
        private Transform armTarget = null;
        private Grabbable grabbedObject;

        private void Start()
        {
            maxRange = armIKSolver.TotalLength; //- (armIKSolver.TotalLength / 5);
            minRange = armIKSolver.TotalLength / 4;
            armTarget = armIKSolver.Target;
        }

        void LateUpdate()
        {
            UpdateArm();
            UpdateHead();
        }

        private void UpdateArm()
        {
            if (grabState == GrabState.NONE)
            {
                if (scheme == ControlScheme.WASD)
                {
                    Vector3 translation = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                    armTarget.transform.Translate(Time.deltaTime * speed * translation);
                    float distanceToTarget = Vector3.Distance(transform.position, armIKSolver.Target.position);
                    if (distanceToTarget > maxRange)
                    {
                        Vector3 dirToTarget = (armTarget.position - transform.position).normalized;
                        armTarget.position = new Vector3(
                            dirToTarget.x * maxRange,
                            armTarget.position.y,
                            dirToTarget.z * maxRange);
                    }

                    if (distanceToTarget < minRange)
                    {
                        Vector3 dirToTarget = (armTarget.position - transform.position).normalized;
                        armTarget.position = new Vector3(
                            dirToTarget.x * minRange,
                            armTarget.position.y,
                            dirToTarget.z * minRange);
                    }
                }
                else if (scheme == ControlScheme.ROTATION)
                {
                    float translation = Input.GetAxis("Vertical");
                    targetRotationRoot.Rotate(new Vector3(0, 1, 0),
                        Mathf.Rad2Deg * Input.GetAxis("Horizontal") * Time.deltaTime);

                    armTarget.Translate(new Vector3(translation * speed * Time.deltaTime, 0, 0));

                    if (armTarget.localPosition.x > maxRange)
                    {
                        armTarget.localPosition = Vector3.right * maxRange;
                    }
                    else if (armTarget.localPosition.x < minRange)
                    {
                        armTarget.localPosition = Vector3.right * minRange;
                    }
                }
            }

            //arm base look at target
            Vector3 direction = armTarget.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            float rotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            armRotationRoot.rotation = Quaternion.Euler(0f, rotationY - 90, 0f);
        }

        private void UpdateHead()
        {
            head.transform.Rotate(new Vector3(0, 0, 1), headRotation - head.transform.eulerAngles.z);
            Grabbable grabTarget = GetGrabTarget();
            if (grabState != GrabState.NONE && grabTarget == null)
            {
                grabState = GrabState.RESET_POSITION;
            }

            switch (grabState)
            {
                case GrabState.NONE:
                    if (Input.GetKeyDown(KeyCode.Space)) grabState = GrabState.MOVE_TO_TARGET;
                    break;
                case GrabState.MOVE_TO_TARGET:
                    MoveToGrabTarget(grabTarget);
                    break;
                case GrabState.GRABBING:
                    break;
                case GrabState.RESET_POSITION:
                    break;
                case GrabState.GRABBED:
                    if (Input.GetKeyDown(KeyCode.Space)) Release();
                    break;
                default:
                    grabState = GrabState.NONE;
                    break;
            }
        }

        private void Release()
        {
        }

        private void MoveToGrabTarget(Grabbable grabTarget)
        {
            if (armTarget.transform.position.y - grabTarget.transform.position.y < 0.01)
            {
                grabState = GrabState.GRABBING;
            }

            armTarget.Translate(Time.deltaTime * grabSpeed * Vector3.down);
        }

        private void ResetPosition()
        {
            //if (Math.Abs(armTarget.transform.localPosition.y -))
        }

        private Grabbable GetGrabTarget()
        {
            RaycastHit hit;
            Vector3 headDirection = head.transform.up;
            if (Physics.Raycast(head.transform.position, headDirection, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(head.transform.position, headDirection * hit.distance,
                    Color.green);
                return hit.transform.GetComponent<Grabbable>();
            }

            return null;
        }
    }
}