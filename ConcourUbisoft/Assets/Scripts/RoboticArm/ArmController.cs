using UnityEngine;

namespace Arm
{
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
        [SerializeField] private Transform grabPoint;
        [SerializeField] private Transform armRotationRoot;
        [SerializeField] float speed = 3f;
        [SerializeField] private float grabSpeed = 3f;
        [SerializeField] private float headRotation = 180;
        private float maxRange;
        private float minRange;
        private float targetStartY;
        private GrabState grabState = GrabState.NONE;
        private Transform armTarget = null;
        private Grabbable grabTarget;

        private void Start()
        {
            maxRange = armIKSolver.TotalLength; //- (armIKSolver.TotalLength / 5);
            minRange = armIKSolver.TotalLength / 4;
            armTarget = armIKSolver.Target;
            targetStartY = armTarget.transform.localPosition.y;
        }

        void LateUpdate()
        {
            UpdateArm();
            UpdateHead();
        }

        private void UpdateArm()
        {
            if (grabState == GrabState.NONE || grabState == GrabState.GRABBED)
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
            
            Vector3 direction = armTarget.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            float rotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            armRotationRoot.rotation = Quaternion.Euler(0f, rotationY - 90, 0f);
        }

        private void UpdateHead()
        {
            head.transform.Rotate(new Vector3(0, 0, 1), headRotation - head.transform.eulerAngles.z);

            switch (grabState)
            {
                case GrabState.NONE:
                    GetGrabTarget();
                    if (Input.GetKeyDown(KeyCode.Space) && grabTarget)
                        grabState = GrabState.MOVE_TO_TARGET;
                    break;
                case GrabState.MOVE_TO_TARGET:
                    MoveToGrabTarget(grabTarget);
                    break;
                case GrabState.GRABBING:


                    break;
                case GrabState.RESET_POSITION:
                    ResetPosition();
                    break;
                case GrabState.GRABBED:
                    if (Input.GetKeyDown(KeyCode.Space))
                        Release();
                    break;
                default:
                    grabState = GrabState.NONE;
                    break;
            }
        }

        private void Release()
        {
            grabTarget.transform.SetParent(null);
            grabState = GrabState.NONE;
            grabTarget.OnRelease();
            grabTarget = null;
        }

        private void MoveToGrabTarget(Grabbable grabTarget)
        {
            if (grabTarget.Constains(grabPoint.position))
            {
                grabState = GrabState.RESET_POSITION;
                grabTarget.transform.SetParent(grabPoint.transform);
                this.grabTarget = grabTarget;
                grabTarget.OnGrab();
            }

            armTarget.Translate(Time.deltaTime * grabSpeed * Vector3.down);
        }

        private void ResetPosition()
        {
            armTarget.Translate(Time.deltaTime * grabSpeed * Vector3.up);
            if (armTarget.transform.localPosition.y >= targetStartY)
            {
                armTarget.transform.localPosition = new Vector3(armTarget.transform.localPosition.x, targetStartY,
                    armTarget.transform.localPosition.z);
                if (grabTarget != null)
                    grabState = GrabState.GRABBED;
                else
                    grabState = GrabState.NONE;
            }
        }

        private void GetGrabTarget()
        {
            RaycastHit hit;
            Vector3 headDirection = head.transform.up;
            if (Physics.Raycast(grabPoint.position, headDirection, out hit, Mathf.Infinity))
            {
                grabTarget = hit.transform.GetComponent<Grabbable>();
                if (grabTarget)
                    Debug.DrawRay(grabPoint.position, headDirection * hit.distance,
                        Color.green);
            }
        }
    }
}