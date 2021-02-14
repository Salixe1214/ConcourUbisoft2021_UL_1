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
        private float maxRange;
        private float minRange;
        private float targetStartY;
        private GrabState grabState = GrabState.NONE;
        private Transform armTarget = null;
        private Pickable grabTarget;
        private Vector3 grabTargetPosition;
        private AudioSource audioSource;

        private void Start()
        {
            maxRange = armIKSolver.TotalLength; //- (armIKSolver.TotalLength / 5);
            minRange = armIKSolver.TotalLength / 4;
            armTarget = armIKSolver.Target;
            targetStartY = armTarget.transform.localPosition.y;
            head.transform.Rotate(new Vector3(0, 0, 1), headRotation - head.transform.eulerAngles.z);
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (controlled)
            {
                UpdateArm();
                UpdateHead();
            }
        }

        private void UpdateArm()
        {
            if (grabState == GrabState.NONE || grabState == GrabState.GRABBED)
            {
                Vector3 translation = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                audioSource.volume = translation.magnitude;
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
                    GetGrabTarget();
                    if ((Input.GetButtonDown("Grab") ||
                         Input.GetButtonDown("GrabControllerXBO") ||
                         Input.GetButtonDown("GrabControllerPS")) && grabTarget)
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

        private void MoveToGrabTarget(Pickable grabTarget)
        {
            if (grabTarget.Constains(grabPoint.position))
            {
                grabState = GrabState.RESET_POSITION;
                grabTarget.transform.SetParent(grabPoint.transform);
                this.grabTarget = grabTarget;
                grabTarget.OnGrab();
            }
            float speed = SpeedFunction(0.2f,0.8f,(armTarget.transform.localPosition.y-grabTargetPosition.y)/(targetStartY-grabTargetPosition.y));
            armTarget.transform.Translate(speed * grabSpeed * Time.deltaTime * Vector3.down);
            audioSource.volume = speed;
        }

        private float SpeedFunction(float a,float b,float x)
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
            float speed = SpeedFunction(0.2f,0.8f,(armTarget.transform.localPosition.y-grabTargetPosition.y)/(targetStartY-grabTargetPosition.y));
            armTarget.transform.Translate(speed * grabSpeed * Time.deltaTime * Vector3.up);
            audioSource.volume = speed;
            if (armTarget.transform.localPosition.y >= targetStartY)
            {
                audioSource.volume = 0;
                armTarget.transform.localPosition = new Vector3(
                    armTarget.transform.localPosition.x,
                    targetStartY,
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
                grabTarget = hit.transform.GetComponent<Pickable>();
                grabTargetPosition = hit.point;
                if (grabTarget)
                    Debug.DrawRay(grabPoint.position, headDirection * hit.distance, Color.green);
            }
        }
    }
}