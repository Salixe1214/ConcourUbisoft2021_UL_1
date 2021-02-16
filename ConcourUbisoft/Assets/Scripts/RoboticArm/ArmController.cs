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
        [SerializeField] private bool controlled;
        [SerializeField] private Transform head;
        [SerializeField] private IKSolver armIKSolver;
        [SerializeField] private Transform grabPoint;
        [SerializeField] private Transform armRotationRoot;
        [SerializeField] float controlSpeed = 3f;
        [SerializeField] private float grabSpeed = 3f;
        [SerializeField] private float headRotation = 180;
        private Transform armTarget = null;
        private Pickable currentPickable = null;
        private Vector3 currentPickableHitPosition;
        private float maxRange;
        private float minRange;
        private float targetStartY;
        private GrabState grabState = GrabState.NONE;

        public float ControlSpeed => controlSpeed;


        private void Start()
        {
            maxRange = armIKSolver.TotalLength;
            minRange = armIKSolver.TotalLength / 4;
            armTarget = armIKSolver.Target;
            targetStartY = armTarget.transform.localPosition.y;
            head.transform.Rotate(new Vector3(0, 0, 1), headRotation - head.transform.eulerAngles.z);
        }

        void Update()
        {
            if (controlled)
            {
                UpdateCurrentPickable();
                UpdateArm();
                UpdateHead();
            }
        }

        private void UpdateArm()
        {
            if (grabState == GrabState.NONE || grabState == GrabState.GRABBED)
            {
                Vector3 translation = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                armTarget.transform.Translate(Time.deltaTime * controlSpeed * translation);
                float distanceToTarget = Vector3.Distance(transform.position, armIKSolver.Target.position);
                if (distanceToTarget > maxRange)
                {
                    Vector3 dirToTarget = (armTarget.position - transform.position).normalized;
                    armTarget.position = new Vector3(
                        transform.position.x + dirToTarget.x * maxRange,
                        armTarget.position.y,
                        transform.position.z + dirToTarget.z * maxRange);
                }

                if (distanceToTarget < minRange)
                {
                    Vector3 dirToTarget = (armTarget.position - transform.position).normalized;
                    armTarget.position = new Vector3(
                        transform.position.x + dirToTarget.x * minRange,
                        armTarget.position.y,
                        transform.position.z + dirToTarget.z * minRange);
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
                    if ((Input.GetButtonDown("Grab") ||
                         Input.GetButtonDown("GrabControllerXBO") ||
                         Input.GetButtonDown("GrabControllerPS")) && currentPickable)
                        grabState = GrabState.MOVE_TO_TARGET;
                    break;
                case GrabState.MOVE_TO_TARGET:
                    MoveToCurrentPickable();
                    break;
                case GrabState.GRABBING:


                    break;
                case GrabState.RESET_POSITION:
                    ResetPosition();
                    break;
                case GrabState.GRABBED:
                    if ((Input.GetButtonDown("Grab") ||
                         Input.GetButtonDown("GrabControllerXBO") ||
                         Input.GetButtonDown("GrabControllerPS")))
                        Release();
                    break;
                default:
                    grabState = GrabState.NONE;
                    break;
            }
        }

        private void Release()
        {
            currentPickable.transform.SetParent(null);
            grabState = GrabState.NONE;
            currentPickable.OnRelease();
            currentPickable = null;
        }

        private void MoveToCurrentPickable()
        {
            if (currentPickable)
            {
                if (currentPickable.Contains(grabPoint.position))
                {
                    grabState = GrabState.RESET_POSITION;
                    currentPickable.transform.SetParent(grabPoint.transform);
                    currentPickable.OnGrab();
                }

                float speed = SpeedFunction(0.2f, 0.8f,
                    (armTarget.transform.localPosition.y - currentPickableHitPosition.y) /
                    (targetStartY - currentPickableHitPosition.y));
                armTarget.transform.Translate(speed * grabSpeed * Time.deltaTime * Vector3.down);
            }
            else
            {
                grabState = GrabState.RESET_POSITION;
            }
        }

        private float SpeedFunction(float a, float b, float x)
        {
            if (x < a)
            {
                return Mathf.SmoothStep(0.1f, 1, x / a);
            }

            if (x < b)
            {
                return 1.0f;
            }

            return Mathf.SmoothStep(1, 0.1f, (x - b) / a);
        }

        private void ResetPosition()
        {
            float speed = SpeedFunction(0.2f, 0.8f,
                (armTarget.transform.localPosition.y - currentPickableHitPosition.y) /
                (targetStartY - currentPickableHitPosition.y));
            armTarget.transform.Translate(speed * grabSpeed * Time.deltaTime * Vector3.up);
            if (armTarget.transform.localPosition.y >= targetStartY)
            {
                armTarget.transform.localPosition = new Vector3(
                    armTarget.transform.localPosition.x,
                    targetStartY,
                    armTarget.transform.localPosition.z);
                if (currentPickable != null)
                    grabState = GrabState.GRABBED;
                else
                    grabState = GrabState.NONE;
            }
        }

        private void UpdateCurrentPickable()
        {
            RaycastHit hit;
            Vector3 headDirection = head.transform.up;
            if (Physics.Raycast(grabPoint.position, headDirection, out hit, Mathf.Infinity))
            {
                currentPickable = hit.transform.GetComponent<Pickable>();
                if (currentPickable)
                {
                    currentPickable.OnHover();
                    Debug.DrawRay(grabPoint.position, headDirection * hit.distance, Color.green);
                }
                else
                {
                    currentPickable = null;
                }
            }
        }
    }
}