using UnityEngine;

namespace Arm
{
    public class ArmController : MonoBehaviour
    {
        [SerializeField] private Controllable controllable;
        [SerializeField] private IKSolver armIKSolver;
        [SerializeField] private Transform armRotationRoot;
        [SerializeField] float controlSpeed = 3f;
        private Transform armTarget = null;
        
        private float maxRange;
        private float minRange;


        public float ControlSpeed => controlSpeed;


        private void Start()
        {
            maxRange = armIKSolver.TotalLength;
            minRange = armIKSolver.TotalLength / 4;
            armTarget = armIKSolver.Target;
        }

        void Update()
        {
            if (controllable.IsControlled)
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
    }
}